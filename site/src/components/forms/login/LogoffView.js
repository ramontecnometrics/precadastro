import { useEffect } from 'react';
import sessionManager from '../../../SessionManager';

const LogoffView = () => {
   
   useEffect(() => {
      sessionManager.logoff();
      window.location = '/adm';
   }, []);

   return null; // não precisa renderizar nada
};

export default LogoffView;
