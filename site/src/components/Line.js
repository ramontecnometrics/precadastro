import React from 'react';
import '../contents/css/page.css';

export default function Line(props) {
   return (
      <div
         className={'line ' + (props.className ? ' ' + props.className : '')}
         style={{
            marginLeft: 0,
            marginRight: 0,
            marginTop: props.marginTop ? props.marginTop : 0,
            marginBottom: props.marginBottom ? props.marginBottom : 0,
            backgroundColor: props.color ? props.color : '#ced4da',
            height: props.height ? props.height : 1,
            width: props.width ? props.width : '100%',
            border: 0,
         }}
         name={props.name}
         id={props.id}
         key={props.key}
         ref={props.ref}
      />
   );
}
