-- Crear la base de datos
CREATE DATABASE BarberiaEstetica;
GO

USE BarberiaEstetica;
GO

-- Crear tabla Roles
CREATE TABLE Roles (
    RolID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);
GO

-- Crear tabla Usuarios
CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    Usuario NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    RolID INT NOT NULL,
    FOREIGN KEY (RolID) REFERENCES Roles(RolID)
);
GO

-- Crear tabla Clientes
CREATE TABLE Clientes (
    ClienteID INT IDENTITY(1,1) PRIMARY KEY,
    Apellido NVARCHAR(100) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20),
    Email NVARCHAR(255)
);
GO

-- Crear tabla Servicios
CREATE TABLE Servicios (
    ServicioID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Precio DECIMAL(10,2) NOT NULL,
    DuracionMinutos INT NOT NULL
);
GO

-- Crear tabla Empleados
CREATE TABLE Empleados (
    EmpleadoID INT IDENTITY(1,1) PRIMARY KEY,
    Apellido NVARCHAR(100) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20),
    HorarioInicio TIME NOT NULL,
    HorarioFin TIME NOT NULL
);
GO

-- Crear tabla EmpleadoServicio (relación muchos a muchos)
CREATE TABLE EmpleadoServicio (
    EmpleadoID INT NOT NULL,
    ServicioID INT NOT NULL,
    PRIMARY KEY (EmpleadoID, ServicioID),
    FOREIGN KEY (EmpleadoID) REFERENCES Empleados(EmpleadoID),
    FOREIGN KEY (ServicioID) REFERENCES Servicios(ServicioID)
);
GO

-- Crear tabla Turnos
CREATE TABLE Turnos (
    TurnoID INT IDENTITY(1,1) PRIMARY KEY,
    ClienteID INT NOT NULL,
    EmpleadoID INT NOT NULL,
    ServicioID INT NOT NULL,
    Fecha DATE NOT NULL,
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),
    FOREIGN KEY (EmpleadoID) REFERENCES Empleados(EmpleadoID),
    FOREIGN KEY (ServicioID) REFERENCES Servicios(ServicioID)
);
GO

-- Crear tabla HistorialTurnos
CREATE TABLE HistorialTurnos (
    HistorialID INT IDENTITY(1,1) PRIMARY KEY,
    TurnoID INT NOT NULL,
    UsuarioID INT NOT NULL,
    FechaCambio DATETIME NOT NULL DEFAULT GETDATE(),
    Accion NVARCHAR(50),
    Comentario NVARCHAR(255),
    FOREIGN KEY (TurnoID) REFERENCES Turnos(TurnoID),
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
);
GO

-- Crear tabla Notificaciones
CREATE TABLE Notificaciones (
    NotificacionID INT IDENTITY(1,1) PRIMARY KEY,
    TurnoID INT NOT NULL,
    Tipo NVARCHAR(50),
    Estado NVARCHAR(20),
    FechaEnvio DATETIME,
    FOREIGN KEY (TurnoID) REFERENCES Turnos(TurnoID)
);
GO

-- Insertar roles básicos
INSERT INTO Roles (Nombre) VALUES 
('Administrador'),
('Empleado'),
('Cliente');
GO

-- Insertar usuario administrador por defecto
INSERT INTO Usuarios (Usuario, Clave, Email, RolID)
VALUES ('admin', 'admin123', 'admin@barberia.com', 1);
GO 