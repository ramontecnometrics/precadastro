import React, { Component, Fragment } from 'react';
import DadosDeUsuarioDeEmpresaView from './Empresa/DadosDeUsuarioDeEmpresaView';
import DadosDaEmpresaView from './Empresa/DadosDaEmpresaView';
import sessionManager from '../SessionManager';

export default function AccountView() {
   let login = sessionManager.getLogin();
   return (
      <Fragment>
         {login.tipoDeAcesso === 'TECNOMETRICS' && <div />}
         {login.tipoDeAcesso === 'EMPRESA' && (
            <Fragment>
               {login.isMasterDaEmpresa && <DadosDaEmpresaView />}
               {!login.isMasterDaEmpresa && <DadosDeUsuarioDeEmpresaView />}
            </Fragment>
         )}
      </Fragment>
   );
}
