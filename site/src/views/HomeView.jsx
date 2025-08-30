import React from 'react';
import sessionManager from '../SessionManager';

import HomeAdministrador from './Administrativo/HomeAdministrador';

export default function HomeView() {
   return <React.Fragment>{sessionManager.isUsuarioAdministrador() && <HomeAdministrador />}</React.Fragment>;
}
