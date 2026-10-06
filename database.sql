CREATE DATABASE IF NOT EXISTS crud_caos CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE crud_caos;
DROP TABLE IF EXISTS clientes;
CREATE TABLE clientes (
 id INT AUTO_INCREMENT PRIMARY KEY,
 nome VARCHAR(120) NOT NULL,
 email VARCHAR(160) NOT NULL,
 idade INT NOT NULL,
 cidade VARCHAR(100) NOT NULL,
 ativo BOOLEAN NOT NULL DEFAULT TRUE,
 criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO clientes (nome,email,idade,cidade,ativo) VALUES
('Ana Martins','ana.martins@exemplo.com',28,'Itajai',1),
('Bruno Costa','bruno.costa@exemplo.com',34,'Blumenau',1),
('Carla Souza','carla.souza@exemplo.com',22,'Joinville',0),
('Diego Lima','diego.lima@exemplo.com',41,'Florianopolis',1),
('Maria Oliveira','maria.oliveira@exemplo.com',31,'Itajai',1),
('Joao Pereira','joao.pereira@exemplo.com',19,'Navegantes',1);
