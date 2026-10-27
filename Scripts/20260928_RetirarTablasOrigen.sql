-- ETAPA 2: ejecutar SOLO en dpt_cedula_2 despues de:
-- 1. Respaldar el estado ACTUAL de dpt_cedula_2 (estructura, datos y rutinas).
-- 2. Ejecutar 20260928_RetirarProcedimientosOrigenModuloPerfil.sql.
-- 3. Confirmar que usuarios y permisos siguen funcionando en la web.
-- Esta operacion elimina 9 y 31 filas auxiliares, respectivamente, segun
-- el recuento previo. El respaldo conserva esos datos si se necesitan luego.

USE dpt_cedula_2;

DROP TABLE tbl_cedula_empresa_usuario_origen;
DROP TABLE tbl_cedula_modulo_perfil_origen;

SELECT TABLE_NAME
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN (
    'tbl_cedula_empresa_usuario_origen',
    'tbl_cedula_modulo_perfil_origen'
  );
