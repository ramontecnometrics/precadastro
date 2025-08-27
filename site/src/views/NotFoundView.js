import React from 'react';
import Filler from '../components/Filler';
import Label from '../components/Label';
import Panel from '../components/Panel';
import { LayoutParams } from '../config/LayoutParams';

export default function NotFoundView() {
   return (
      <Panel contentOnCenter={true}>
         <div style={{ display: 'block', fontSize: 22 }}>
            <Filler height={30} />
            <div style={{ textAlign: 'center' }}>
               <Label>{'Nada encontrado aqui'}</Label>
            </div>
            <Filler height={150} />
            <br />
            <br />
            <br />
            <div style={{ textAlign: 'center', width: '100%', height: '70%', display: 'flex' }}>
               <img
                  src={LayoutParams.imgLogoBrancoMarcaDagua}
                  alt='logo-sensetrack.png'
                  style={{ maxWidth: '50%', margin: 'auto', opacity: 0.3 }}
               />
            </div>
         </div>
      </Panel>
   );
}
