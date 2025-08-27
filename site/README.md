# Sistema de Pré-Cadastro

Um sistema moderno e responsivo para pré-cadastro de usuários desenvolvido em React, com formulário completo organizado em seções.

## 🚀 Funcionalidades

- Formulário completo de pré-cadastro organizado em seções
- Interface moderna e responsiva
- Design adaptável para dispositivos móveis
- Validação de campos obrigatórios
- Upload de arquivos (foto)
- Checkbox para status ativo
- Textarea para observações
- Selects com opções pré-definidas

## 📋 Seções do Formulário

### **Dados Básicos**
- **Celular** (Telefone) - ✅ Obrigatório
- **Nome** (Texto) - ✅ Obrigatório  
- **Gênero** (Select) - ✅ Obrigatório

### **Informações Adicionais**
- **Data de Nascimento** (Date picker) - ❌ Opcional
- **Estado Civil** (Select) - ❌ Opcional
- **Profissão** (Texto) - ❌ Opcional
- **Nacionalidade** (Select) - ✅ Obrigatório
- **Telefone** (Telefone) - ❌ Opcional
- **Email** (Email) - ❌ Opcional
- **CPF** (Texto numérico) - ❌ Opcional
- **RG** (Texto) - ❌ Opcional
- **CNH** (Texto) - ❌ Opcional

### **Endereço**
- **País** (Select) - ❌ Opcional
- **CEP** (Texto numérico) - ❌ Opcional
- **Bairro** (Texto) - ❌ Opcional
- **Estado** (Select) - ❌ Opcional
- **Logradouro** (Texto) - ❌ Opcional
- **Número** (Texto numérico) - ❌ Opcional
- **Complemento** (Texto) - ❌ Opcional
- **Cidade** (Texto) - ❌ Opcional

### **Configurações**
- **Foto** (Upload de arquivo) - ❌ Opcional
- **Ativo** (Checkbox) - ❌ Opcional

### **Observações**
- **Observações** (Textarea) - ❌ Opcional

## 🛠️ Tecnologias Utilizadas

- React 19.1.1
- React Router DOM 6.8.0
- CSS3 com Gradientes e Animações
- HTML5 Semântico
- JavaScript ES6+
- CSS Grid e Flexbox para layout responsivo

## 🚀 Como Executar

### Pré-requisitos

- Node.js (versão 14 ou superior)
- npm ou yarn

### Instalação

1. Clone o repositório ou navegue até o diretório do projeto:
```bash
cd pre-cadastro
```

2. Instale as dependências:
```bash
npm install
```

3. Execute o projeto em modo de desenvolvimento:
```bash
npm start
```

4. Abra [http://localhost:3000](http://localhost:3000) no seu navegador para visualizar o projeto.

## 🛣️ Rotas Disponíveis

- **/** - Formulário de pré-cadastro
- **/adm/login** - Tela de login administrativa
- **/adm** - Painel administrativo (após login)
- **/adm/usuario** - Área administrativa de usuários (em desenvolvimento)
- **/*** - Redireciona para a página inicial

## 📱 Responsividade

O projeto é totalmente responsivo e funciona perfeitamente em:
- Desktop (layout em duas colunas)
- Tablet (layout adaptativo)
- Smartphone (layout em coluna única)

## 🎨 Design

- Interface moderna com gradientes
- Seções bem definidas com bordas e cores
- Animações suaves
- Cores harmoniosas (roxo/azul)
- Tipografia clara e legível
- Feedback visual nos campos de entrada
- Scrollbar personalizada
- Layout em grid para formulários em linha

## 📦 Scripts Disponíveis

- `npm start` - Executa o projeto em modo de desenvolvimento
- `npm run build` - Cria a versão de produção
- `npm test` - Executa os testes
- `npm run eject` - Ejetar configurações (irreversível)

## 🔧 Estrutura do Projeto

```
pre-cadastro/
├── public/
│   ├── index.html
│   └── manifest.json
├── src/
│   ├── views/
│   │   ├── Home.js         # Painel administrativo
│   │   ├── PreCadastro.js  # Formulário de pré-cadastro
│   │   ├── Usuario.js      # Área administrativa de usuários
│   │   └── Login.js        # Tela de login administrativa
│   ├── App.js              # Configuração de rotas
│   ├── App.css             # Estilos modernos
│   ├── index.js            # Ponto de entrada
│   └── index.css           # Estilos globais
├── package.json
└── README.md
```

## 🎯 Características Especiais

- **Sistema de Rotas**: Navegação entre páginas com React Router
- **Autenticação**: Sistema de login administrativo
- **Painel Administrador**: Área restrita para administradores
- **Layout em Seções**: Formulário organizado em seções lógicas
- **Campos em Linha**: Alguns campos são exibidos lado a lado
- **Upload de Arquivo**: Campo para upload de foto com estilização
- **Checkbox Personalizado**: Checkbox "Ativo" com design customizado
- **Selects Estilizados**: Dropdowns com ícone de seta personalizado
- **Textarea**: Campo para observações com altura ajustável
- **Scroll Interno**: Container com scroll para formulários longos
- **Validação**: Campos obrigatórios marcados com asterisco (*)
- **Mostrar/Ocultar Senha**: Toggle para visualizar senha no login

## 📄 Licença

Este projeto está sob a licença MIT.

## 👨‍💻 Desenvolvimento

Para contribuir com o projeto:

1. Faça um fork do repositório
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

---

Desenvolvido com ❤️ usando React
