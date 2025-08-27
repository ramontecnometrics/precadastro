import React from 'react';
import { Form } from 'react-bootstrap';
import '../contents/css/text-input.css';

export default function TextInput(props) {
   return (
      <Form.Control
         type='text'
         className={'form-control text-input' + (props.className ? ' ' + props.className : '')}
         defaultValue={props.defaultValue}
         onChange={(e) => {
            let oldSelectionStart = e.target.selectionStart;
            let oldSelectionEnd = e.target.selectionEnd;

            if (props.upperCase) {
               e.target.value = ('' + e.target.value).toUpperCase();
            }
            if (props.lowerCase) {
               e.target.value = ('' + e.target.value).toLowerCase();
            }

            e.target.selectionStart = oldSelectionStart;
            e.target.selectionEnd = oldSelectionEnd;

            if (props.onChange) {
               props.onChange(e.target.value);
            }
         }}
         maxLength={props.maxLength}
         style={props.style}
         name={props.name}
         id={props.id}
         key={props.key}
         ref={props.ref}
         readOnly={props.readOnly}
         placeholder={props.placeholder}
      />
   );
}
