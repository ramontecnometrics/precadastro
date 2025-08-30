import React, { useState, useRef, useEffect } from 'react';
import Calendar from 'react-calendar';
import { InputGroup, Form, Modal } from 'react-bootstrap';
import IconButton from './IconButton';
import { faCalendarAlt } from '@fortawesome/free-regular-svg-icons';
import styled from 'styled-components';
import * as moment from 'moment';
import { showError } from '../components/Messages';
import { replaceAll } from '../utils/Functions';
import '../contents/css/date-input.css';
import { LayoutParams } from '../config/LayoutParams';
import Text from './Text';

export default function DateInput(props) {
   const [state, setState] = useState(() => {
      let date = new Date();
      if (props.defaultValue && moment(props.defaultValue).isValid()) {
         date = moment(props.defaultValue).toDate();
      }
      return {
         date: date,
         dataDigitada: moment(date).format('DD/MM/YYYY'),
         mostrarCalendario: false,
      };
   });

   const inputRef = useRef(null);

   const setDateFromCalendar = (value) => {
      let date = value;
      setState((prev) => ({ ...prev, date: date }));

      if (!date) {
         if (inputRef.current) {
            inputRef.current.value = null;
         }
         setState((prev) => ({ ...prev, dataDigitada: null }));
      } else {
         if (inputRef.current) {
            inputRef.current.value = moment(date).format('DD/MM/YYYY');
         }
         setState((prev) => ({ ...prev, dataDigitada: moment(date).format('DD/MM/YYYY') }));
      }

      if (props.onChange) {
         props.onChange(date);
      }
   };

   const closeCalendar = () => {
      setTimeout(() => {
         setState((prev) => ({ ...prev, mostrarCalendario: false }));
      }, 100);
   };

   const handleInputChange = (e) => {
      setState((prev) => ({ ...prev, dataDigitada: e.target.value }));
   };

   const handleInputBlur = () => {
      if (state.dataDigitada) {
         let d = null;
         let dataString = null;

         dataString = replaceAll(state.dataDigitada, '/', '');
         let dia = dataString.substr(0, 2);
         let mes = dataString.substr(2, 2);
         let ano = dataString.substr(4, 4);
         d = new Date(parseInt(ano), parseInt(mes) - 1, parseInt(dia));

         var d2 = moment(d);
         if (d2.isValid() && dataString === d2.format(replaceAll(replaceAll('DD/MM/YYYY', '-', ''), '/', ''))) {
            if (props.onChange) {
               props.onChange(d2.toDate());
            }
            inputRef.current.value = d2.format('DD/MM/YYYY');
         } else {
            showError(`${state.dataDigitada} não é uma data válida`);
            inputRef.current.value = moment(state.date ? state.date : new Date()).format('DD/MM/YYYY');
         }
      } else {
         setState((prev) => ({ ...prev, date: null, dataDigitada: null }));
         if (props.onChange) {
            props.onChange(null);
         }
      }
   };

   const handleCalendarClick = () => {
      setState((prev) => ({ ...prev, mostrarCalendario: true }));
   };

   const handleClearDate = () => {
      setDateFromCalendar(null);
      closeCalendar();
   };

   const handleToday = () => {
      setDateFromCalendar(new Date());
      closeCalendar();
   };

   let inputBackgroundColor = {
      backgroundColor: props.readOnly ? '#e9ecef' : null,
      paddingLeft: 6,
      paddingRight: 6,
   };

   return (
      <DateInputStyled style={{ display: 'flex' }} name={props.name} id={props.id} key={props.key}>
         <InputGroup style={{ backgroundColor: 'transparent', flexWrap: 'nowrap' }}>
            <Form.Control
               type='text'
               className={'date-input' + (props.className ? ' ' + props.className : '')}
               ref={inputRef}
               defaultValue={props.defaultValue ? moment(state.date).format('DD/MM/YYYY') : null}
               style={{ ...props.style, ...inputBackgroundColor }}
               onChange={handleInputChange}
               readOnly={props.readOnly}
               maxLength={10}
               onBlur={handleInputBlur}
            />
            {!props.readOnly && (
               <InputGroup.Text
                  className='hide-when-readonly'
                  style={{
                     padding: '1px 6px 1px 6px',
                     height: props.style && props.style.height ? props.style.height : 38,
                     width: 40,
                     justifyContent: 'center',
                  }}
                  onClick={handleCalendarClick}
               >
                  <div style={{ height: 24, width: 30, display: 'table-cell', paddingTop: 1 }}>
                     <IconButton
                        className={'date-input-icon'}
                        style={{ color: LayoutParams.colors.corDoTemaPrincipal, fontSize: 20 }}
                        icon={faCalendarAlt}
                     />
                  </div>
               </InputGroup.Text>
            )}
         </InputGroup>

         <Modal
            show={state.mostrarCalendario}
            onHide={() => {}}
            onKeyDown={(e) => {
               if (e.keyCode === 27) closeCalendar();
            }}
            aria-labelledby='contained-modal-title-vcenter'
            centered
            size='sm'
         >
            <Modal.Body
               style={{
                  padding: '15px 15px 15px 15px',
                  fontSize: 15,
                  width: 'fit-content',
               }}
            >
               {state.mostrarCalendario && (
                  <Calendar
                     onChange={setDateFromCalendar}
                     value={state.date}
                     onClickDay={() => {
                        closeCalendar();
                     }}
                     prevLabel={'<'}
                     nextLabel={'>'}
                     prev2Label={null}
                     next2Label={null}
                     showFixedNumberOfWeeks={false}
                     calendarType={'gregory'}
                  />
               )}

               <div style={{ display: 'flex', width: '100%', justifyContent: 'flex-end' }}>
                  <div
                     style={{
                        textAlign: 'right',
                        color: '#999',
                        cursor: 'pointer',
                        marginRight: 10,
                        fontSize: 14,
                        display: 'flex',
                        justifyContent: 'flex-end',
                        width: 140,
                     }}
                     onClick={handleClearDate}
                  >
                     <div
                        style={{
                           display: 'table-cell',
                           paddingLeft: 10,
                           paddingRight: 10,
                           width: 75,
                        }}
                     >
                        <Text>Limpar</Text>
                     </div>
                  </div>
                  <div
                     style={{
                        color: '#999',
                        textAlign: 'right',
                        marginRight: 10,
                        display: 'flex',
                        justifyContent: 'flex-start',
                        width: '100%',
                     }}
                  >
                     <div
                        style={{
                           cursor: 'pointer',
                           marginRight: 10,
                           display: 'flex',
                        }}
                        onClick={closeCalendar}
                     >
                        <div
                           style={{
                              display: 'table-cell',
                              paddingLeft: 10,
                              paddingRight: 10,
                           }}
                        >
                           <Text>Cancelar</Text>
                        </div>
                     </div>
                  </div>
                  <div
                     style={{
                        textAlign: 'right',
                        color: '#999',
                        cursor: 'pointer',
                        marginRight: 10,
                        fontSize: 14,
                        display: 'flex',
                        justifyContent: 'flex-end',
                        minWidth: 55,
                     }}
                     onClick={handleToday}
                  >
                     <div
                        style={{
                           display: 'table-cell',
                           paddingLeft: 10,
                           paddingRight: 10,
                        }}
                     >
                        <Text>Hoje</Text>
                     </div>
                  </div>
               </div>
            </Modal.Body>
         </Modal>
      </DateInputStyled>
   );
}

const DateInputStyled = styled.div`
   .dropdown-toggle {
      box-shadow: none;
   }
`;
