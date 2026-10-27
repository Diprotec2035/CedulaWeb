-- ETAPA 1: ejecutar SOLO en dpt_cedula_2 con respaldo actualizado.
-- dpt_cedula conserva estos procedimientos; NO ejecutar alli.
-- En dpt_cedula_2 no se hallaron llamadas en la web, API, app Android ni
-- desde otras rutinas. Performance Schema no registro ejecuciones, lo que
-- NO demuestra que nunca hayan sido usados por clientes externos.
-- Despues de ejecutar, probar Modulos x perfil antes de tocar las tablas.

USE dpt_cedula_2;

DROP PROCEDURE IF EXISTS SP_INS_MODULO_PERFIL;
DROP PROCEDURE IF EXISTS SP_SEL_MODULO_PERFIL;
DROP PROCEDURE IF EXISTS SP_UPD_MODULO_PERFIL;

SELECT ROUTINE_NAME
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME IN (
    'SP_INS_MODULO_PERFIL',
    'SP_SEL_MODULO_PERFIL',
    'SP_UPD_MODULO_PERFIL'
  );

SELECT ROUTINE_NAME
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_DEFINITION LIKE '%cedula%origen%';
