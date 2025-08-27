import React from 'react';
import { Link } from 'react-router-dom';
import '../App.css';

function Home() {
  return (
    <div className="App">
      <div className="container">
        <header className="header">
          <h1>Sistema de Gestão</h1>
          <p>Bem-vindo ao sistema de gestão integrada</p>
        </header>

        <main className="main-content">
          <div className="home-content">
            <div className="welcome-section">
              <h2>Escolha uma opção:</h2>
              
              <div className="menu-options">
                <Link to="/precadastro" className="menu-option">
                  <div className="option-icon">📝</div>
                  <h3>Pré-Cadastro</h3>
                  <p>Realizar pré-cadastro de usuários</p>
                </Link>
                
                                 <Link to="/adm/usuario" className="menu-option">
                   <div className="option-icon">👥</div>
                   <h3>Usuários</h3>
                   <p>Gerenciar usuários cadastrados</p>
                 </Link>
                
                {/* <div className="menu-option disabled">
                  <div className="option-icon">📊</div>
                  <h3>Relatórios</h3>
                  <p>Visualizar relatórios e estatísticas</p>
                  <span className="coming-soon">Em breve</Text>
                </div>
                
                <div className="menu-option disabled">
                  <div className="option-icon">⚙️</div>
                  <h3>Configurações</h3>
                  <p>Configurar sistema</p>
                  <span className="coming-soon">Em breve</Text>
                </div> */}
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}

export default Home;
