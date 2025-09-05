import React from 'react';
import { Routes, Route, Navigate, BrowserRouter as HashRouter } from 'react-router-dom';
import PreCadastroView from './views/PreCadastroView';
import AvaliacaoClinicaView from './views/AvaliacaoClinicaView';
import AnamneseView from './views/AnamneseView';
import sessionManager from './SessionManager';
import MainLayout from './components/MainLayout';
import TermosDeUsoView from './views/TermosDeUsoView';
import AboutView from './views/AboutView';
import NotFoundView from './views/NotFoundView';
import LayoutAdministrador from './views/Administrativo/LayoutAdministrador';
import LoginView from './components/forms/login/LoginView'; 
import menus from './Menus';
import RecoverPasswordView from './components/forms/login/RecoverPasswordView';
import LogoffView from './components/forms/login/LogoffView';
import ErrorView from './views/ErrorView';
import 'bootstrap/dist/css/bootstrap.min.css';
import './contents/css/page.css';
import './contents/css/custom-bootstrap.css';

function App() {

   let result = null;

   if (sessionManager.isAuthenticated()) {   
      if (!sessionManager.aceitouTermosDeUso()) {
         result = (
            <HashRouter>
               <MainLayout
                  menuItems={menus.getMenusParaAceiteDeTermosDeUso(sessionManager)}
                  mostrarDadosDaConta={false}
               >
                  <Routes>
                     <Route path='/adm' exact element={<TermosDeUsoView rows={20} />} />
                     <Route path='/adm/login' element={<LoginView tipoDeAcesso='ADM' />} />
                     <Route path='/adm/logoff' element={<LogoffView />} />
                     <Route path='/adm/recoverpassword' element={<RecoverPasswordView />} />
                     <Route path='/adm/error' element={<ErrorView />} />
                     <Route path='/adm/about' element={<AboutView />} />
                     <Route path='*' element={<NotFoundView />} />
                  </Routes>
               </MainLayout>
            </HashRouter>
         );
      } else if (sessionManager.isUsuarioAdministrador()) {      
         if (window.location.pathname === '/' || window.location.pathname === '/cadastro') {
            result = (
               <PreCadastroView />
            );
         } else 
         if (window.location.pathname === '/avaliacaoclinica') {
            result = (
               <AvaliacaoClinicaView />
            );
         } else if (window.location.pathname === '/anamnese') {
            result = (
               <AnamneseView />
            );
         } else {               
            result = (
               <LayoutAdministrador
                  mostrarDadosDaConta={true}
               />
            );
         }

         
      }
   } else {
      result = (
         <React.Fragment>
            <HashRouter>
               <Routes>
                  <Route path="/" exact element={<PreCadastroView />} />
                  <Route path='/cadastro' exact element={<PreCadastroView />} />
                  <Route path='/anamnese' exact element={<AnamneseView />} />
                  <Route path='/adm' exact element={<LoginView tipoDeAcesso='ADM' />} />
                  <Route path='/adm/login' element={<LoginView tipoDeAcesso='ADM' />} />
                  <Route path='/adm/error' element={<ErrorView />} />
                  <Route path='/adm/logoff' element={<LogoffView />} />
                  <Route path="*" element={<Navigate to="/" replace />} />
               </Routes>
            </HashRouter>
         </React.Fragment>
      );
   }

   return result;
}

export default App;
