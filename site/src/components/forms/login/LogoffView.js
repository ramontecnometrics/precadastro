import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import sessionManager from '../../../SessionManager';

const LogoffView = () => {
   const navigate = useNavigate();

   useEffect(() => {
      sessionStorage.removeItem('usuario');

      if (sessionManager.isAuthenticated() && sessionManager.isUsuarioTecnometricsSupervisor()) {
         navigate('/supervisor', { replace: true });
      } else if (sessionManager.isAuthenticated() && sessionManager.isUsuarioTecnometrics()) {
         navigate('/adm', { replace: true });
      } else {
         navigate('/', { replace: true });
      }
   }, [navigate]);

   return null; // não precisa renderizar nada
};

export default LogoffView;
