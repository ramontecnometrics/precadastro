class SessionManager {
   getLogin() {
      let result = JSON.parse(localStorage.getItem('usuario'));
      return result;
   }

   setLogin(usuario) {      
      localStorage.setItem('usuario', JSON.stringify(usuario));
   }

   logoff() {
      localStorage.removeItem('usuario');
   }

   isAuthenticated() {
      let usuario = JSON.parse(localStorage.getItem('usuario'));
      return usuario && usuario.token && window.location.pathname !== '/login';
   }

   temAcessoARotina(rotina) {
      let result = this.isAuthenticated() && this.getLogin().rotinasAcessiveis.some((i) => i.id === rotina);
      return result;
   }

   isUsuarioAdministrador() {
      let result = false;
      let usuario = JSON.parse(localStorage.getItem('usuario'));
      if (usuario && usuario.token && window.location.pathname !== '/adm/login') {
         result = usuario.tipoDeAcesso === 'ADM';
      }
      return result;
   } 
  
   aceitouTermosDeUso() {
      let result = false;
      let usuario = JSON.parse(localStorage.getItem('usuario'));
      if (usuario && usuario.token && window.location.pathname !== '/login') {
         result = usuario.aceitouTermosDeUso;
      }
      return result;
   }
 
}

const sessionManager = new SessionManager();
export default sessionManager;