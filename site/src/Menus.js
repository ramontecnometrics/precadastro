import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { faBars, faCogs, faFile, faHome, faInfoCircle, faListAlt, faStore, faTools, faUser } from '@fortawesome/free-solid-svg-icons';
import sessionManager from './SessionManager';

class Menus {
   constructor() {
      this.temAcessoARotina = this.temAcessoARotina.bind(this);
   }

   temAcessoARotina = (rotina) => {
      let result = rotina ? sessionManager.temAcessoARotina(rotina) : false;
      return result;
   };

   getMenusParaAceiteDeTermosDeUso = () => {
      let result = [
         { key: 10, label: 'Início', icon: faHome, fontWeight: 500, route: '/adm', enabled: true },
         {
            key: 30,
            label: 'Sobre',
            icon: faInfoCircle,
            fontWeight: 500,
            enabled: true,
            route: '/adm/about',
         },
      ];
      return result;
   };

   getMenusAdministrador = () => {
      let result = [
         { key: 10, label: 'Início', icon: faHome, fontWeight: 500, route: '/adm', enabled: true },
         {
            key: 20,
            label: 'Cadastro',
            icon: faListAlt,
            fontWeight: 500,
            route: null,
            enabled: true,
            subMenu: [
               {
                  key: 1011,
                  label: 'Perfil de Usuário',
                  icon: faListAlt,
                  route: '/adm/1011',
                  enabled: this.temAcessoARotina(1011),
               },
               {
                  key: 1021,
                  label: 'Usuários',
                  icon: faIdCard,
                  route: '/adm/1021',
                  enabled: this.temAcessoARotina(1021),
               },
               {
                  key: 1061,
                  label: 'Formulários',
                  icon: faFile,
                  route: '/adm/1061',
                  enabled: this.temAcessoARotina(1061),
               },
               {
                  key: 1071,
                  label: 'Unidades',
                  icon: faStore,
                  route: '/adm/1071',
                  enabled: this.temAcessoARotina(1071),
               },
            ],
         },
         {
            key: 15,
            label: 'Leads',
            icon: faUser,
            fontWeight: 500,
            route: '/adm/1051',
            enabled: this.temAcessoARotina(1051),
         },
         {
            key: 20,
            label: 'Configurações',
            icon: faCogs,
            fontWeight: 500,
            enabled: true,
            subMenu: [
               {
                  key: 1041,
                  label: 'Parâmetros do Sistema',
                  icon: faCogs,
                  route: '/adm/1041',
                  enabled: this.temAcessoARotina(1041),
               },
               {
                  key: 1031,
                  label: 'Rotinas do Sistema',
                  icon: faListAlt,
                  route: '/adm/1031',
                  enabled: this.temAcessoARotina(1031),
               },

               {
                  key: 1381,
                  label: 'Termo',
                  icon: faListAlt,
                  route: '/adm/1381',
                  enabled: this.temAcessoARotina(1381),
               },
            ],
         },
         {
            key: 25,
            label: 'Manutenção',
            icon: faTools,
            fontWeight: 500,
            enabled: true,
            subMenu: [
               {
                  key: 9001,
                  label: 'Logs',
                  icon: faBars,
                  route: '/adm/9001',
                  enabled: this.temAcessoARotina(9001),
               },
               {
                  key: 1501,
                  label: 'Auditoria',
                  icon: faBars,
                  route: '/adm/1501',
                  enabled: this.temAcessoARotina(1501),
               },
            ],
         },
         {
            key: 30,
            label: 'Sobre',
            icon: faInfoCircle,
            fontWeight: 500,
            enabled: true,
            route: '/adm/about',
         },
      ];
      return result;
   };
}
const menus = new Menus();
export default menus;
