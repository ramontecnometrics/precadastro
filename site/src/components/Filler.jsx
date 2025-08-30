import React from 'react';

export default function Filler({ height, width, marginTop, marginBottom, color }) {
   return (
      <React.Fragment>
         <div
            style={{
               marginLeft: 0,
               marginRight: 0,
               marginTop: marginTop ? marginTop : 0,
               marginBottom: marginBottom ? marginBottom : 0,
               borderColor: color,
               width: width,
               height: height,
            }}
         />
      </React.Fragment>
   );
};
