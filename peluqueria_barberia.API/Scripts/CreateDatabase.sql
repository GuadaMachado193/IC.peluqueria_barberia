-- Crear la base de datos
CREATE DATABASE BarberiaEstetica;
GO

USE BarberiaEstetica;
GO

-- Tabla de Roles
CREATE TABLE Roles (
    RolID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL UNIQUE -- Admin, Empleado, Cliente
);
GO

-- Insertar roles básicos
INSERT INTO Roles (Nombre) VALUES 
('admin'),
('empleado'),
('cliente');
GO

-- Tabla de Estados de Turno
CREATE TABLE EstadosTurno (
    EstadoID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);
GO

-- Insertar estados de turno
INSERT INTO EstadosTurno (Nombre) VALUES 
('pendiente'),
('en_proceso'),
('completado'),
('cancelado');
GO

-- Tabla de Usuarios del sistema
CREATE TABLE Usuarios (
    UsuarioID INT PRIMARY KEY IDENTITY(1,1),
    Usuario NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    Apellido NVARCHAR(100),
    Nombre NVARCHAR(100),
    Telefono NVARCHAR(20),
    Estado NVARCHAR(20) DEFAULT 'activo',
    RolID INT NOT NULL,
    FOREIGN KEY (RolID) REFERENCES Roles(RolID)
);
GO

-- Tabla de Clientes
CREATE TABLE Clientes (
    ClienteID INT PRIMARY KEY IDENTITY(1,1),
    Apellido NVARCHAR(100) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20),
    Email NVARCHAR(255)
);
GO

-- Tabla de Servicios
CREATE TABLE Servicios (
    ServicioID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Precio DECIMAL(10,2) NOT NULL,
    DuracionMinutos INT NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'activo'
);
GO

-- Tabla de Empleados
CREATE TABLE Empleados (
    EmpleadoID INT PRIMARY KEY IDENTITY(1,1),
    Apellido NVARCHAR(100) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20),
    HorarioInicio TIME NOT NULL,
    HorarioFin TIME NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'activo'
);
GO

-- Tabla de Turnos
CREATE TABLE Turnos (
    TurnoID INT PRIMARY KEY IDENTITY(1,1),
    ClienteID INT NOT NULL,
    EmpleadoID INT NOT NULL,
    ServicioID INT NOT NULL,
    Fecha DATE NOT NULL,
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    Estado NVARCHAR(20) NOT NULL,
    Notas NVARCHAR(MAX),
    FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),
    FOREIGN KEY (EmpleadoID) REFERENCES Empleados(EmpleadoID),
    FOREIGN KEY (ServicioID) REFERENCES Servicios(ServicioID),
    FOREIGN KEY (Estado) REFERENCES EstadosTurno(Nombre)
);
GO

-- Tabla de Historial de cambios de turnos
CREATE TABLE HistorialTurnos (
    HistorialID INT PRIMARY KEY IDENTITY(1,1),
    TurnoID INT NOT NULL,
    UsuarioID INT NOT NULL,
    FechaCambio DATETIME DEFAULT GETDATE(),
    Accion NVARCHAR(50),
    Comentario NVARCHAR(255),
    FOREIGN KEY (TurnoID) REFERENCES Turnos(TurnoID),
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
);
GO

-- Tabla de Notificaciones
CREATE TABLE Notificaciones (
    NotificacionID INT PRIMARY KEY IDENTITY(1,1),
    TurnoID INT NOT NULL,
    Tipo NVARCHAR(50),
    Estado NVARCHAR(20),
    FechaEnvio DATETIME,
    FOREIGN KEY (TurnoID) REFERENCES Turnos(TurnoID)
);
GO

-- Crear índices
CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
CREATE INDEX IX_Usuarios_RolID ON Usuarios(RolID);
CREATE INDEX IX_Turnos_ClienteID ON Turnos(ClienteID);
CREATE INDEX IX_Turnos_EmpleadoID ON Turnos(EmpleadoID);
CREATE INDEX IX_Turnos_Fecha ON Turnos(Fecha);
CREATE INDEX IX_Turnos_Estado ON Turnos(Estado);
CREATE INDEX IX_Turnos_Fecha_Empleado ON Turnos(Fecha, EmpleadoID);
GO

-- Crear vistas
CREATE VIEW vw_TurnosDetallados AS
SELECT 
    t.TurnoID,
    c.Nombre + ' ' + c.Apellido AS ClienteNombre,
    e.Nombre + ' ' + e.Apellido AS EmpleadoNombre,
    s.Nombre AS ServicioNombre,
    t.Fecha,
    t.HoraInicio,
    t.HoraFin,
    t.Estado,
    t.Notas,
    t.FechaModificacion
FROM Turnos t
JOIN Clientes c ON t.ClienteID = c.ClienteID
JOIN Empleados e ON t.EmpleadoID = e.EmpleadoID
JOIN Servicios s ON t.ServicioID = s.ServicioID;
GO

-- Crear procedimientos almacenados
CREATE PROCEDURE sp_CrearTurno
    @ClienteID INT,
    @EmpleadoID INT,
    @ServicioID INT,
    @Fecha DATE,
    @HoraInicio TIME
AS
BEGIN
    DECLARE @HoraFin TIME;
    
    SELECT @HoraFin = DATEADD(MINUTE, s.DuracionMinutos, @HoraInicio)
    FROM Servicios s
    WHERE s.ServicioID = @ServicioID;
    
    INSERT INTO Turnos (ClienteID, EmpleadoID, ServicioID, Fecha, HoraInicio, HoraFin, Estado)
    VALUES (@ClienteID, @EmpleadoID, @ServicioID, @Fecha, @HoraInicio, @HoraFin, 'pendiente');
    
    INSERT INTO HistorialTurnos (TurnoID, UsuarioID, Accion, Comentario)
    VALUES (SCOPE_IDENTITY(), 1, 'Creado', 'Turno creado');
END;
GO

CREATE PROCEDURE sp_ActualizarEstadoTurno
    @TurnoID INT,
    @NuevoEstado NVARCHAR(20),
    @UsuarioID INT,
    @Comentario NVARCHAR(255) = NULL
AS
BEGIN
    UPDATE Turnos
    SET Estado = @NuevoEstado,
        FechaModificacion = GETDATE()
    WHERE TurnoID = @TurnoID;
    
    INSERT INTO HistorialTurnos (TurnoID, UsuarioID, Accion, Comentario)
    VALUES (@TurnoID, @UsuarioID, 'Modificado', @Comentario);
END;
GO

-- Crear triggers
CREATE TRIGGER tr_Turnos_Update
ON Turnos
AFTER UPDATE
AS
BEGIN
    UPDATE Turnos
    SET FechaModificacion = GETDATE()
    FROM Turnos t
    INNER JOIN inserted i ON t.TurnoID = i.TurnoID;
END;
GO

CREATE TRIGGER tr_Turnos_Insert
ON Turnos
AFTER INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Turnos t
        INNER JOIN inserted i ON t.EmpleadoID = i.EmpleadoID
        AND t.Fecha = i.Fecha
        AND (
            (i.HoraInicio BETWEEN t.HoraInicio AND t.HoraFin)
            OR (i.HoraFin BETWEEN t.HoraInicio AND t.HoraFin)
        )
    )
    BEGIN
        RAISERROR ('El empleado ya tiene un turno asignado en ese horario', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- Insertar datos de ejemplo
-- Insertar un administrador por defecto
INSERT INTO Usuarios (Usuario, Clave, Email, Nombre, Apellido, RolID)
VALUES ('admin', 'admin123', 'admin@barberia.com', 'Administrador', 'Sistema', 1);
GO

-- Insertar algunos servicios de ejemplo
INSERT INTO Servicios (Nombre, Descripcion, Precio, DuracionMinutos)
VALUES 
('Corte de cabello', 'Corte de cabello básico', 1500.00, 30),
('Barba', 'Arreglo de barba', 1000.00, 20),
('Corte y barba', 'Corte de cabello y arreglo de barba', 2000.00, 45);
GO 