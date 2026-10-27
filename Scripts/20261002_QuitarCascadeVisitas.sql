/*
    dpt_cedula_2: quita ON DELETE / ON UPDATE CASCADE de las tablas de visitas.

    En Inventario ninguna llave foranea usa CASCADE: la base impide borrar un
    usuario, dispositivo o empresa que tenga registros asociados. En Cedula,
    tbl_registro_visita y tbl_empresa_visita usaban CASCADE, por lo que borrar
    un usuario, dispositivo o empresa eliminaba su historial de movimientos.
    Ni la web, ni la API, ni ningun procedimiento borran esas tablas padre,
    por lo que el funcionamiento actual no cambia.
    Las llaves se recrean con el mismo nombre, sin CASCADE.

    Ejecutar POR BLOQUES, con respaldo.
*/

USE dpt_cedula_2;

/* BLOQUE 1 - Diagnostico: debe listar 4 llaves con CASCADE. */
SELECT TABLE_NAME, CONSTRAINT_NAME, DELETE_RULE, UPDATE_RULE
FROM information_schema.REFERENTIAL_CONSTRAINTS
WHERE CONSTRAINT_SCHEMA = 'dpt_cedula_2'
  AND (DELETE_RULE = 'CASCADE' OR UPDATE_RULE = 'CASCADE');

/* BLOQUE 2 - Recrear las llaves sin CASCADE. */
ALTER TABLE `tbl_registro_visita` DROP FOREIGN KEY `FK_REGISTRO_VISITA:DISPOSITIVO`;
ALTER TABLE `tbl_registro_visita` ADD CONSTRAINT `FK_REGISTRO_VISITA:DISPOSITIVO` FOREIGN KEY (`DIS_ID`) REFERENCES `tbl_dispositivo` (`DIS_ID`);

ALTER TABLE `tbl_registro_visita` DROP FOREIGN KEY `FK_REGISTRO_VISITA:EMPRESA`;
ALTER TABLE `tbl_registro_visita` ADD CONSTRAINT `FK_REGISTRO_VISITA:EMPRESA` FOREIGN KEY (`EMP_RUT`) REFERENCES `tbl_empresa` (`EMP_RUT`);

ALTER TABLE `tbl_registro_visita` DROP FOREIGN KEY `FK_REGISTRO_VISITA:USUARIO`;
ALTER TABLE `tbl_registro_visita` ADD CONSTRAINT `FK_REGISTRO_VISITA:USUARIO` FOREIGN KEY (`USU_RUT`) REFERENCES `tbl_usuario` (`USU_RUT`);

ALTER TABLE `tbl_empresa_visita` DROP FOREIGN KEY `FK_EMPRESA_VISITA:EMPRESA`;
ALTER TABLE `tbl_empresa_visita` ADD CONSTRAINT `FK_EMPRESA_VISITA:EMPRESA` FOREIGN KEY (`EMP_RUT`) REFERENCES `tbl_empresa` (`EMP_RUT`);

/* BLOQUE 3 - Verificacion: no debe devolver filas. */
SELECT TABLE_NAME, CONSTRAINT_NAME, DELETE_RULE, UPDATE_RULE
FROM information_schema.REFERENTIAL_CONSTRAINTS
WHERE CONSTRAINT_SCHEMA = 'dpt_cedula_2'
  AND (DELETE_RULE = 'CASCADE' OR UPDATE_RULE = 'CASCADE');
