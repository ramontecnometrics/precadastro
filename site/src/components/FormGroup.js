import React from 'react';
import { FormGroup as BootstrapFormGroup } from 'react-bootstrap';
import Filler from './Filler';

export default function FormGroup(props) {
   return (
      <BootstrapFormGroup {...props} style={{ marginBottom: 8 }}>
         {props.children}
         
      </BootstrapFormGroup>
   );
}
