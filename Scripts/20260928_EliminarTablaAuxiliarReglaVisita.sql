/*
    Elimina exclusivamente la tabla auxiliar de reglas de dpt_cedula_2.
    Requisitos ya comprobados durante la migracion:
    - los indicadores estan en tbl_regla;
    - los ocho procedimientos migrados no consultan esta tabla;
    - triggers y vistas no la consultan;
    - la web supero pruebas de lectura y escritura;
    - existe respaldo SQL previo.

    La diferencia de contrato entre la API y SP_SEL_REGLAS_PERFIL
    (P_REG_VIGENTE/REG_VIGENTE vs P_REG_ESTADO/REG_ESTADO) es independiente
    de esta tabla y queda pendiente de resolver.

    DROP TABLE hace COMMIT implicito en MySQL. No hay rollback automatico.
*/

DROP TABLE dpt_cedula_2.tbl_cedula_regla_visita;
