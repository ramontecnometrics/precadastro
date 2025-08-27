import React from 'react';
import '../App.css';

function Usuario() {
  return (
    <div className="App">
      <div className="container">
        <header className="header">
          <h1>Cadastro de Usuários</h1>
          <p>Área administrativa para gerenciamento de usuários</p>
        </header>

        <main className="main-content">
          <div className="admin-content">
            <div className="admin-section">
              <h2>Funcionalidade em Desenvolvimento</h2>
              <div className="development-notice">
                <div className="notice-icon">🚧</div>
                <h3>Página em Construção</h3>
                <p>Esta funcionalidade está sendo desenvolvida e estará disponível em breve.</p>
                <div className="features-preview">
                  <h4>Funcionalidades planejadas:</h4>
                  <ul>
                    <li>✅ Listagem de usuários cadastrados</li>
                    <li>✅ Cadastro de novos usuários</li>
                    <li>✅ Edição de dados de usuários</li>
                    <li>✅ Exclusão de usuários</li>
                    <li>✅ Filtros e busca avançada</li>
                    <li>✅ Exportação de dados</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}

export default Usuario;
