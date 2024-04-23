-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 23-04-2024 a las 03:29:05
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `IdContrato` int(11) NOT NULL,
  `FechaInicio` datetime NOT NULL,
  `FechaFinalizacion` datetime NOT NULL,
  `MontoAlquiler` decimal(10,2) NOT NULL,
  `Estado` tinyint(1) NOT NULL,
  `IdInquilino` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`IdContrato`, `FechaInicio`, `FechaFinalizacion`, `MontoAlquiler`, `Estado`, `IdInquilino`, `IdInmueble`) VALUES
(6, '2020-02-02 12:12:00', '2022-04-03 23:13:00', 23.00, 1, 1, 3),
(14, '2016-03-02 21:12:00', '2020-07-03 03:03:00', 324423.00, 1, 6, 1),
(15, '2003-02-02 02:02:00', '2014-01-31 02:03:00', 32.00, 1, 7, 4);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `IdInmueble` int(11) NOT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Uso` varchar(50) NOT NULL,
  `Tipo` varchar(50) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Superficie` int(11) NOT NULL,
  `Latitud` decimal(10,0) DEFAULT NULL,
  `Longitud` decimal(10,0) DEFAULT NULL,
  `Valor` decimal(10,0) DEFAULT NULL,
  `IdPropietario` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`IdInmueble`, `Direccion`, `Uso`, `Tipo`, `Ambientes`, `Superficie`, `Latitud`, `Longitud`, `Valor`, `IdPropietario`) VALUES
(1, 'Belgrano 123', 'Residencial', 'Departamento', 2, 131, 6, 1, 34324, 6),
(3, '9 DE JULIO 922', 'Comercial', 'Local Comercial', 3, 1, 2, 2, 4564, 3),
(4, 'Las palmeras', 'Residencial', 'Casa', 5, 1, 123, 32, 2342, 8),
(7, 'ayacucho 1234', 'Comercial', 'Departamento', 2, 3, 23, 234, 20000, 3);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `IdInquilino` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` varchar(50) NOT NULL,
  `Telefono` varchar(40) NOT NULL,
  `Email` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`IdInquilino`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`) VALUES
(1, 'Pepe', 'Funes', '12345', '98765', 'ale@gmail.com'),
(6, 'Lautaro', 'Martinez', '999', '888', 'toro@gmail.com'),
(7, 'Juan Pablo', 'Diaz', '1231', '4901823', 'juanpablodiaz1234@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `IdPago` int(11) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `Monto` decimal(10,2) NOT NULL,
  `Detalle` varchar(255) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 0,
  `IdContrato` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`IdPago`, `FechaPago`, `Monto`, `Detalle`, `Estado`, `IdContrato`) VALUES
(1, '2024-04-10 14:21:11', 23.00, 'Mes abril', 0, 6);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `IdPropietario` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` varchar(50) NOT NULL,
  `Telefono` varchar(50) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `Clave` varchar(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`IdPropietario`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Clave`) VALUES
(3, 'mauro', 'torres', '123123545', '123454', 'mauro@gmail.com', '1232'),
(6, 'Pepe', 'Mujica', '1', '1280937', 'pepitomuj@gmail.com', '1231231'),
(7, 'Tamara', 'Demichelis', '123123123', '1203912', 'tamara@gmail.com', '12'),
(8, 'Tiziana', 'Corvalan', '48128383', '26647777777', 'tizi@gmail.com', '123');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `IdUsuario` int(11) NOT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Apellido` varchar(50) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Clave` varchar(100) DEFAULT NULL,
  `AvatarUrl` varchar(1000) DEFAULT NULL,
  `Rol` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`IdUsuario`, `Nombre`, `Apellido`, `Email`, `Clave`, `AvatarUrl`, `Rol`) VALUES
(4, 'ola', 'mundo', 'mauro@gmail.com', 'R4az/8y0ULOTuylBZ48KBeBConbAoQV3g1vAYmGiWX0=', '/Uploads\\avatar_4.jpg', 1),
(7, 'Tiziana', 'Corvalan', 'tizi@gmail.com', 'R4az/8y0ULOTuylBZ48KBeBConbAoQV3g1vAYmGiWX0=', '/Uploads\\avatar_7.jpg', 1),
(9, 'pepe', 'feo', 'pepe@gmail.com', 'R4az/8y0ULOTuylBZ48KBeBConbAoQV3g1vAYmGiWX0=', NULL, 2),
(11, 'Sergio', 'Massa', 'sergio@gmail.com', 'R4az/8y0ULOTuylBZ48KBeBConbAoQV3g1vAYmGiWX0=', NULL, 2);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`IdContrato`),
  ADD KEY `FK_IdInquilino` (`IdInquilino`),
  ADD KEY `FK_IdInmueble` (`IdInmueble`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`IdInmueble`),
  ADD KEY `FK_IdPropietario` (`IdPropietario`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`IdInquilino`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`IdPago`),
  ADD KEY `IdContrato` (`IdContrato`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`IdPropietario`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`IdUsuario`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `IdContrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `IdInmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `IdInquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `IdPago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `IdPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FK_IdInmueble` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`IdInmueble`),
  ADD CONSTRAINT `FK_IdInquilino` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilinos` (`IdInquilino`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `FK_IdPropietario` FOREIGN KEY (`IdPropietario`) REFERENCES `propietarios` (`IdPropietario`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`IdContrato`) REFERENCES `contratos` (`IdContrato`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
