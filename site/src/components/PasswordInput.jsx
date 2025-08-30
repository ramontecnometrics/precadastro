import IconButton from './IconButton';
import React, { useState } from 'react';
import { FormControl, InputGroup } from 'react-bootstrap';
import '../contents/css/text-input.css';
import { LayoutParams } from '../config/LayoutParams';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

const corDoIcone = LayoutParams.colors.corSecundaria;

export default function PasswordInput(props) {
   const [showPassword, setShowPassword] = useState(false);

   return (
      <InputGroup style={{ flexWrap: 'nowrap' }}>
         <FormControl
            type={showPassword ? 'text' : 'password'}
            className={'text-input' + (props.className ? ' ' + props.className : '')}
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            onChange={(e) => props.onChange(e.target.value)}
            placeholder={props.placeholder}
            defaultValue={props.defaultValue}
            readOnly={props.readOnly}
            value={props.value}
         />

         {(props.appendText || props.appendIcon || !props.readOnly) && (
            <InputGroup.Text
               className='hide-when-readonly'
               title={props.appendTitle}
               style={{ cursor: props.onAppendClick || !props.readOnly ? 'pointer' : undefined }}
            >
               {/* Botão customizado que já existia */}
               {props.appendIcon && (
                  <div style={{ marginTop: 0 }} onClick={props.onAppendClick}>
                     <IconButton
                        style={{
                           fontSize: 20,
                           paddingTop: 3,
                           color: corDoIcone,
                        }}
                        icon={props.appendIcon}
                     />
                  </div>
               )}
               {props.appendText && <span>{props.appendText}</span>}

               {/* Toggle mostrar/esconder senha */}
               {!props.readOnly && (
                  <div
                     style={{ marginLeft: props.appendIcon ? 5 : 0, borderLeft: props.appendIcon ?  '1px solid lightgray' : null}}
                     onClick={() => setShowPassword((prev) => !prev)}
                  >
                     <IconButton
                        style={{
                           fontSize: 20,
                           marginLeft: props.appendIcon ? 5 : 0,
                           paddingTop: 3,
                           color: corDoIcone,
                        }}
                        icon={showPassword ? faEyeSlash : faEye}
                     />
                  </div>
               )}
            </InputGroup.Text>
         )}
      </InputGroup>
   );
}
