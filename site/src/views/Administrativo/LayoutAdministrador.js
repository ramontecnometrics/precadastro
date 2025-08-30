import React from 'react';
import { BrowserRouter as HashRouter, Route, Routes } from 'react-router-dom';
import LogoffView from '../../components/forms/login/LogoffView';
import NotFoundView from '../NotFoundView';
import ErrorView from '../ErrorView';
import MainLayout from '../../components/MainLayout';
import HomeView from '../HomeView';
import PerfilDeUsuarioView from './PerfilDeUsuarioView';
import RotinaDoSistemaView from '../RotinaDoSistemaView';
import ParametroDoSistemaView from './ParametroDoSistemaView';
import CidadeView from '../CidadeView';
import UsuarioAdministradorView from './UsuarioAdministradorView';
import TermoDeUsoView from './TermoDeUsoView';
import AboutView from '../AboutView';
import TermosDeUsoView from '../TermosDeUsoView';
import AuditoriaView from '../AuditoriaView';
import menus from '../../Menus';
import LoginView from '../../components/forms/login/LoginView';
import LogView from '../LogView';
import LeadView from './LeadView';
import FormularioView from './FormularioView';
import UnidadeView from './UnidadeView';

export default function LayoutAdministrador({ mostrarDadosDaConta }) {
   return (
      <HashRouter>
         <MainLayout menuItems={menus.getMenusAdministrador()} mostrarDadosDaConta={mostrarDadosDaConta}>
            <Routes>
               <Route path='/adm' element={<HomeView />} />
               <Route path='/adm/login' element={<LoginView />} />
               <Route path='/adm/logoff' element={<LogoffView />} />
               <Route path='/adm/error' element={<ErrorView />} />
               <Route path='/adm/about' element={<AboutView />} />
               <Route path='/adm/1381' element={<TermoDeUsoView />} />
               <Route path='/adm/90003' element={<TermosDeUsoView />} />
               <Route path='/adm/9001' element={<LogView />} />
               <Route path='/adm/1501' element={<AuditoriaView />} />
               
               <Route path='/adm/1011' element={<PerfilDeUsuarioView />} />
               <Route path='/adm/1021' element={<UsuarioAdministradorView />} />
               <Route path='/adm/1061' element={<FormularioView />} />
               <Route path='/adm/1031' element={<RotinaDoSistemaView />} />
               <Route path='/adm/1041' element={<ParametroDoSistemaView />} />
               <Route path='/adm/1051' element={<LeadView />} />
               <Route path='/adm/1071' element={<UnidadeView />} />
               <Route path='/adm/1111' element={<CidadeView />} />
               <Route path='*' element={<NotFoundView />} />
            </Routes>
         </MainLayout>
      </HashRouter>
   );
}
