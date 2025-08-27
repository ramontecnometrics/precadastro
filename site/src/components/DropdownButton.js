import React, { useState, useRef, useImperativeHandle, forwardRef } from 'react';
import { Dropdown } from 'react-bootstrap';
import { LayoutParams } from '../config/LayoutParams';
import '../contents/css/button-input.css';

const DropdownButton = forwardRef((props, ref) => {
   const [state, setState] = useState({
      text: props.text,
      disabled: false,
   });

   const buttonRef = useRef(null);

   useImperativeHandle(ref, () => ({
      focus: () => {
         if (buttonRef.current) {
            buttonRef.current.focus();
         }
      }
   }));

   const defaultStyle = {
      backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
      borderBlockColor: LayoutParams.colors.corSecundaria,
      color: LayoutParams.colors.corSecundaria,
      borderColor: LayoutParams.colors.corSecundaria,
      fontSize: 16,
      height: 36,
      textAlign: 'center',
      margin: 0,
      padding: '0 4px 0 4px',
      overFlow: 'hidden',
   };

   return (
      <Dropdown>
         <Dropdown.Toggle
            className={'button-input' + (props.className ? ' ' + props.className : '')}
            style={{ ...defaultStyle, ...props.style }}
            title={props.title}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={buttonRef}
            disabled={state.disabled || props.disabled}
            block
            type={props.submit ? 'submit' : 'button'}
         >
            {state.text}
         </Dropdown.Toggle>

         <Dropdown.Menu>
             {props.children}
         </Dropdown.Menu>
      </Dropdown>
   );
});

DropdownButton.displayName = 'DropdownButton';

export default DropdownButton;
