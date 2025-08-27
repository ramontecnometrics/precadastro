import React from 'react';
import { HashRouter, Route, Routes } from 'react-router-dom';
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

export default function LayoutAdministrador({ mostrarDadosDaConta }) {
   return (
      <HashRouter>
         <MainLayout menuItems={menus.getMenusAdministrador()} mostrarDadosDaConta={mostrarDadosDaConta}>
            <Routes>
               <Route path='/' element={<HomeView />} />
               <Route path='/login' element={<LoginView />} />
               <Route path='/logoff' element={<LogoffView />} />
               <Route path='/error' element={<ErrorView />} />
               <Route path='/about' element={<AboutView />} />
               <Route path='/1011' element={<PerfilDeUsuarioView />} />
               <Route path='/1021' element={<UsuarioAdministradorView />} />
               <Route path='/1031' element={<RotinaDoSistemaView />} />
               <Route path='/1041' element={<ParametroDoSistemaView />} />
               <Route path='/1111' element={<CidadeView />} />
               <Route path='/1381' element={<TermoDeUsoView />} />
               <Route path='/90003' element={<TermosDeUsoView />} />
               <Route path='/9001' element={<LogView />} />
               <Route path='/1501' element={<AuditoriaView />} />
               <Route path='*' element={<NotFoundView />} />
            </Routes>
         </MainLayout>
      </HashRouter>
   );
}
