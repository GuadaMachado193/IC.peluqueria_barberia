USE BarberiaEstetica;
GO

-- Verificar si existe la tabla EmpleadoServicio antes de eliminarla
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmpleadoServicio')
BEGIN
    -- 1. Eliminar la tabla EmpleadoServicio
    -- Primero eliminamos las referencias en la tabla Turnos
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Turnos_Servicios')
    BEGIN
        ALTER TABLE Turnos
        DROP CONSTRAINT FK_Turnos_Servicios;
    END
    GO

    -- Eliminamos la tabla EmpleadoServicio
    DROP TABLE EmpleadoServicio;
END
GO

-- 2. Agregar columna Descripcion a la tabla Empleados si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Empleados') AND name = 'Descripcion')
BEGIN
    ALTER TABLE Empleados
    ADD Descripcion NVARCHAR(MAX);
END
GO

-- 3. Crear o reemplazar vista para administrador
IF EXISTS (SELECT * FROM sys.views WHERE name = 'VistaAdministrador')
    DROP VIEW VistaAdministrador;
GO

CREATE VIEW VistaAdministrador AS
SELECT 
    t.TurnoID,
    c.Apellido + ', ' + c.Nombre AS Cliente,
    e.Apellido + ', ' + e.Nombre AS Empleado,
    s.Nombre AS Servicio,
    t.Fecha,
    t.HoraInicio,
    t.HoraFin,
    t.Estado,
    t.FechaModificacion,
    c.Telefono AS TelefonoCliente,
    c.Email AS EmailCliente,
    e.Telefono AS TelefonoEmpleado,
    s.Precio,
    s.DuracionMinutos,
    e.Descripcion AS DescripcionEmpleado
FROM 
    Turnos t
    INNER JOIN Clientes c ON t.ClienteID = c.ClienteID
    INNER JOIN Empleados e ON t.EmpleadoID = e.EmpleadoID
    INNER JOIN Servicios s ON t.ServicioID = s.ServicioID;
GO

-- 4. Crear o reemplazar vista para clientes
IF EXISTS (SELECT * FROM sys.views WHERE name = 'VistaCliente')
    DROP VIEW VistaCliente;
GO

CREATE VIEW VistaCliente AS
SELECT 
    t.TurnoID,
    c.Apellido + ', ' + c.Nombre AS Cliente,
    e.Apellido + ', ' + e.Nombre AS Empleado,
    s.Nombre AS Servicio,
    t.Fecha,
    t.HoraInicio,
    t.HoraFin,
    t.Estado,
    t.FechaModificacion,
    c.Telefono AS TelefonoCliente,
    c.Email AS EmailCliente,
    e.Telefono AS TelefonoEmpleado,
    s.Precio,
    s.DuracionMinutos,
    e.Descripcion AS DescripcionEmpleado
FROM 
    Turnos t
    INNER JOIN Clientes c ON t.ClienteID = c.ClienteID
    INNER JOIN Empleados e ON t.EmpleadoID = e.EmpleadoID
    INNER JOIN Servicios s ON t.ServicioID = s.ServicioID;
GO

-- 5. Crear o reemplazar procedimiento almacenado para cancelar turno
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'CancelarTurno')
    DROP PROCEDURE CancelarTurno;
GO

CREATE PROCEDURE CancelarTurno
    @TurnoID INT,
    @UsuarioID INT,
    @Comentario NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar si el turno existe
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE TurnoID = @TurnoID)
        BEGIN
            RAISERROR('El turno especificado no existe.', 16, 1);
            RETURN;
        END

        -- Actualizar estado del turno
        UPDATE Turnos
        SET Estado = 'Cancelado',
            FechaModificacion = GETDATE()
        WHERE TurnoID = @TurnoID;

        -- Registrar en historial
        INSERT INTO HistorialTurnos (TurnoID, UsuarioID, Accion, Comentario)
        VALUES (@TurnoID, @UsuarioID, 'Cancelado', @Comentario);

        -- Crear notificación
        INSERT INTO Notificaciones (TurnoID, Tipo, Estado, FechaEnvio)
        VALUES (@TurnoID, 'Cancelación', 'Pendiente', GETDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 6. Crear o reemplazar procedimiento almacenado para eliminar turno
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'EliminarTurno')
    DROP PROCEDURE EliminarTurno;
GO

CREATE PROCEDURE EliminarTurno
    @TurnoID INT,
    @UsuarioID INT,
    @Comentario NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar si el turno existe
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE TurnoID = @TurnoID)
        BEGIN
            RAISERROR('El turno especificado no existe.', 16, 1);
            RETURN;
        END

        -- Registrar en historial antes de eliminar
        INSERT INTO HistorialTurnos (TurnoID, UsuarioID, Accion, Comentario)
        VALUES (@TurnoID, @UsuarioID, 'Eliminado', @Comentario);

        -- Eliminar notificaciones asociadas
        DELETE FROM Notificaciones WHERE TurnoID = @TurnoID;

        -- Eliminar registros del historial
        DELETE FROM HistorialTurnos WHERE TurnoID = @TurnoID;

        -- Eliminar el turno
        DELETE FROM Turnos WHERE TurnoID = @TurnoID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO 