# Guia de Instalação e Configuração - Ubuntu Server 22.04 (AWS)

## 1. Atualizar pacotes do sistema
```bash
sudo apt update && sudo apt upgrade -y
sudo apt install gnupg2 wget ca-certificates -y
reboot
```

---

## 2. Conexão SSH
```bash
# Conectar ao servidor via SSH
ssh -i "nome_da_chave.pem" nome_do_usuario@nome_do_servidor
ssh -i "chave-padrao.pem" ubuntu@54.233.198.13
```

### Habilitar acesso por usuário e senha
```bash
vi /etc/ssh/sshd_config.d/gestia-sftp.conf
```
Adicionar:
```
Match User gestia
PasswordAuthentication yes
AuthenticationMethods password keyboard-interactive
```
Salvar e reiniciar:
```bash
systemctl restart ssh
```

---

## 3. Configurar data e hora
```bash
# Listar timezones
timedatectl list-timezones | grep Paulo

# Definir fuso horário
timedatectl set-timezone America/Sao_Paulo
```

---

## 4. Desabilitar atualizações automáticas
```bash
vi /etc/apt/apt.conf.d/20auto-upgrades
```
Definir:
```
APT::Periodic::Update-Package-Lists "0";
APT::Periodic::Unattended-Upgrade "0";
```

---

## 5. Instalar .NET 8
```bash
apt-get install -y dotnet-sdk-8.0
```

---

## 6. Instalar PostgreSQL 17
```bash
apt install -y postgresql-common
/usr/share/postgresql-common/pgdg/apt.postgresql.org.sh
apt install postgresql-17
apt install postgresql-client-17
```

### Conectar no banco
```sql
sudo -u postgres psql
```

### Criar bases de dados
```sql
create database precadastro_drhair;
create database precadastro_drhair_log;
```

### Criar extensões
```sql
create extension if not exists pg_stat_statements;
create extension if not exists dblink;
```

### Criar usuário
```sql
create user usergestia with encrypted password 'SENHA-XYZ';
-- Comando para alterar senha se necessário
-- alter user usergestia with password 'SENHA-XYZ';
```

### Alterar o proprietário da base
```sql
alter database precadastro_drhair owner to usergestia;
alter database precadastro_drhair_log owner to usergestia;
```

### Configurar timezone do PostgreSQL
```sql
-- Ver timezones disponíveis
-- select * from pg_timezone_names;

alter database precadastro_drhair set timezone to 'Brazil/East';
alter database precadastro_drhair_log set timezone to 'Brazil/East';
select pg_reload_conf();
```

---

## 7. Instalar e configurar Nginx
```bash
apt-get install nginx
systemctl start nginx
systemctl status nginx
```

---

## 8. Criar usuário para manutenção
```bash
adduser gestia
# Comando para remover case seja necessário recriar
# > deluser --remove-home gestia
# Comando para alterar senha ccase seja necessário
```

---

## 9. Criar estrutura de pastas das aplicações
```bash
mkdir -p /opt/app/precadastro_drhair/{setup,api,api-bkp,api-temp,site,arquivos}
chown -R gestia /opt/app/precadastro_drhair
chmod +rwX -R /opt/app/precadastro_drhair
```

---

## 10. Criar serviço da API (systemd)

### Script de inicialização
```bash
vi /opt/app/precadastro_drhair/api/api.sh
```
Conteúdo:
```bash
#!/bin/bash
dotnet /opt/app/precadastro_drhair/api/api.dll
```

### Arquivo de serviço
```bash
vi /etc/systemd/system/precadastro_drhair.service
```
Conteúdo:
```
[Unit]
Description=Serviços da API do Pre-cadastro Dr Hair
After=network.target 

[Service]
WorkingDirectory=/opt/app/precadastro_drhair/api
ExecStart=/bin/bash /opt/app/precadastro_drhair/api/api.sh
SyslogIdentifier=PrecadastroDrHairApi
Restart=on-failure
RestartSec=20s

[Install]
WantedBy=multi-user.target
```

### Ativar serviço
```bash
systemctl enable precadastro_drhair
systemctl daemon-reload
```

---

## 11. Configuração do Nginx para publicar o site
```bash
vi /etc/nginx/sites-enabled/default
```
Exemplo:
```
log_format access_log_80 '$remote_addr - $status "$request"';

server {
    listen 80;  
    server_name h-cadastrodrhair.gestia.net.br;

    error_page 404 /404.html;
    error_page 502 /502.html;

    location / {
        root /opt/app/precadastro_drhair/site;
        index index.html;
        try_files $uri $uri/ =404;
    }

    location /404.html {
        return 404 "<H1>404 Not Found</H1>";
    }

    location /502.html {
        return 502 "<H1>Servico indisponivel</H1>";
    }
}
```

### Testar e reiniciar
```bash
nginx -t
systemctl restart nginx
systemctl status nginx
```

---

## 12. Configurar firewall (UFW)
```bash
ufw allow ssh
ufw allow 80
ufw enable
ufw reload
ufw status numbered
```

---

## 13. Permissões sudo sem senha
```bash
vi /etc/sudoers.d/gestia
```
Conteúdo:
```
gestia ALL=NOPASSWD: /bin/systemctl start precadastro_drhair.service,                      /bin/systemctl stop precadastro_drhair.service,                      /bin/systemctl status precadastro_drhair.service,                      /bin/systemctl restart precadastro_drhair.service
```
