import React from 'react';

export const ViewController = ({ selected, v1, v2, v3, v4, v5, v6 }) => {
   return (
      <div
         id='div-view-controller'
         style={{ height: '100%', display: 'flex', flexDirection: 'column', overflow: 'auto', marginBottom: 5 }}
      >
         <div
            style={{
               height: selected === 1 ? '100%' : 0.0001,
               width: selected === 1 ? '100%' : 0.0001,
               overflow: 'clip',
            }}
         >
            {v1}
         </div>
         <div
            style={{
               height: selected === 2 ? '100%' : 0.0001,
               width: selected === 2 ? '100%' : 0.0001,
               overflow: 'clip',
            }}
         >
            {v2}
         </div>
         <div
            style={{
               height: selected === 3 ? '100%' : 0.0001,
               width: selected === 3 ? '100%' : 0.0001,
               overflow: 'clip',
            }}
         >
            {v3}
         </div>
         <div
            style={{
               height: selected === 4 ? '100%' : 0.0001,
               width: selected === 4 ? '100%' : 0.0001,
               overflow: 'clip',
            }}
         >
            {v4}
         </div>

         <div
            style={{
               height: selected === 5 ? '100%' : 0.0001,
               width: selected === 5 ? '100%' : 0.0001,
               overflow: 'clip',
            }}
         >
            {v5}
         </div>

         <div
            style={{
               height: selected === 6 ? '100%' : 0.0001,
               width: selected === 6 ? '100%' : 0.0001,
               overflow: 'clip',
            }}
         >
            {v6}
         </div>
      </div>
   );
};
