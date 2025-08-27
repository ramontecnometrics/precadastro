import React from 'react';
import { Form } from 'react-bootstrap';
import styled from 'styled-components';
import '../contents/css/checkbox-input.css';

function CheckBox(props) {
   return (
      <CheckBoxStyled className={'checkbox-input' + (props.className ? ' ' + props.className : '')}>
         <Form.Label
            style={{
               display: 'flex',
               textAlign: 'left',
               cursor: 'pointer',
               marginBottom: 0,
               marginTop: props.marginTop,
            }}
         >
            <Form.Check
               type='checkbox'
               name={props.name}
               defaultChecked={props.defaultChecked}
               checked={props.checked}
               onChange={(e) => {
                  if (props.onChange) {
                     props.onChange(e.target.checked ? true : false);
                  }
               }}
               style={{  ...props.style, paddingTop: 2 }}
               disabled={props.disabled}
            />
            {props.label && <span>&nbsp;{props.label}</span>}
         </Form.Label>
      </CheckBoxStyled>
   );
}

const CheckBoxStyled = styled.div`
   .form-label {
      display: flex;
   }

   .form-check {
      display: flex;
      text-align: center;
      padding-left: 3px;
      padding-right: 5px;
      gap: 5px;
   }

   .form-check-input {
      margin: auto;
      cursor: pointer;
      height: 16px;
      width: 16px;
   }

   span:disabled,
   span[disabled='disabled'],
   span[disabled] {
      cursor: not-allowed !important;
      margin-left: 5px;
   }
`;

export default CheckBox;
