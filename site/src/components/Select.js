import React, { useState, useEffect, useRef } from 'react';
import { FormControl, Modal, InputGroup } from 'react-bootstrap';
import { faCaretUp, faSearch, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import IconButton from './IconButton';
import { inputToUpper } from '../utils/Functions';
import api from '../utils/Api';
import styled from 'styled-components';
import '../contents/css/select.css';
import { create_UUID } from '../utils/Functions';
import { LayoutParams } from '../config/LayoutParams';
import Text from './Text';

const corDoIcone = LayoutParams.colors.corDosBotoesDoFormularioPadrao;

export default function Select(props) {
   const [state, setState] = useState(() => {
      const noDropDown = props.noDropDown || props.readOnly;
      return {
         inserindo: false,
         searchText: null,
         oneItemSelected: noDropDown && props.defaultValue ? props.defaultValue : null,
         lastFilter: null,
         previewOptions: null,
         noDropDown: noDropDown,
      };
   });

   const name = props.name ? props.name : create_UUID();
   const selectRef = useRef(null);
   const inputRef = useRef(null);
   const waitingDataRef = useRef(false);
   const changeTimerRef = useRef(null);
   const changedRef = useRef(false);
   const lastSelectedOptionRef = useRef(null);
   const previewFocusRef = useRef(0);

   useEffect(() => {
      let result = props.defaultValue;
      if (!result && props.asws) {
         if (props.options && props.options.length === 1) {
            onSelect(props.options[0]);
         }
      }
      if (!result && props.acceptZero) {
         if (props.options) {
            let zeroOptions = props.options.filter((i) => props.getKeyValue(i) === 0);
            if (zeroOptions.length > 0) {
               onSelect(zeroOptions[0]);
            }
         }
      }
   }, []);

   const getOptions = () => {
      let result = [];
      if (!props.options && !state.noDropDown) {
         console.warn('Opções não definidas para o componente ' + props.name + '.');
      } else {
         result = props.filter ? props.options.filter(props.filter) : props.options;
      }
      return result;
   };

   const getDefault = () => {
      let result = props.defaultValue;
      if (!result || (result === 0 && props.acceptZero)) {
         let options = props.filter ? props.options.filter(props.filter) : props.options;
         if (options) {
            if (props.asws && options.length === 1) {
               result = props.getKeyValue(options[0]);
               onSelect(options[0]);
            }
            if (props.allowEmpty === false && options.length > 0 && (result === null || result === undefined)) {
               result = props.getKeyValue(options[0]);
               onSelect(options[0]);
            }
         }
      }
      return result;
   };

   const showModal = () => {
      setState(prev => ({ ...prev, inserindo: true }));
   };

   const onSelect = (item) => {
      if (
         item &&
         lastSelectedOptionRef.current &&
         props.getKeyValue(item) === props.getKeyValue(lastSelectedOptionRef.current)
      ) {
         return;
      }
      lastSelectedOptionRef.current = item;
      if (props.onSelect) {
         props.onSelect(item);
      }
   };

   const aoSelecionar = (item, novo) => {
      if (item) {
         if (novo && !state.noDropDown) {
            props.options.push(item);
         }
         onSelect(item);
         if (!state.noDropDown) {
            if (props.updateOptions) {
               props.updateOptions(props.options);
            }
            selectRef.current.value = props.getKeyValue(item);
         } else {
            if (props.getDescription && inputRef.current) {
               inputRef.current.value = props.getDescription(item);
            }
            setState(prev => ({ ...prev, oneItemSelected: item }));
         }

         setState(prev => ({ ...prev, inserindo: false }));
      }
   };

   const aoCancelar = () => {
      setState(prev => ({ ...prev, inserindo: false }));
   };

   const getSearchText = () => {
      return state.searchText;
   };

   const clear = () => {
      let f = () => {
         inputRef.current.value = null;
         setState(prev => ({
            ...prev,
            oneItemSelected: null,
            searchText: null,
         }));
         onSelect(null);
      };

      if (props.beforeClear) {
         props.beforeClear().then(f);
      } else {
         f();
      }
   };

   const filter = (text) => {
      if (state.noDropDown && props.getFilterUrl) {
         if (text && text.length >= 3) {
            if (text !== state.lastFilter) {
               if (waitingDataRef.current) {
                  return;
               }

               waitingDataRef.current = true;

               api
                  .getAll(props.getFilterUrl(text))
                  .then((result) => {
                     if (props.filter) {
                        result = props.filter(result.items);
                     }
                     setState(prev => ({ ...prev, previewOptions: result.items }));
                  })
                  .finally(() => {
                     waitingDataRef.current = false;
                  });
            }
         } else {
            setState(prev => ({ ...prev, previewOptions: null }));
         }
      }
   };

   const getReadOnlyColor = () => {
      return props.readOnlyColor ? props.readOnlyColor : '#ffff';
   };

   const setFocus = (i) => {
      if (!state.previewOptions) {
         return;
      }

      if (i <= 0) {
         return;
      }

      if (!previewFocusRef.current) {
         previewFocusRef.current = 0;
      }

      let item = document.getElementById(name + '__' + (previewFocusRef.current + i));
      if (item) {
         previewFocusRef.current = previewFocusRef.current + i;
         if (previewFocusRef.current > state.previewOptions.length) {
            previewFocusRef.current = state.previewOptions.length;
         }
         item.focus();
      }
   };

   const _default = getDefault();
   let defaultDescription = null;
   let defaultKeyValue = null;

   if (_default !== null && _default !== undefined) {
      if (props.getDescription) {
         try {
            defaultDescription = props.getDescription(_default);
         } catch (e) {
            console.error(e);
            console.error('Valor:', props.defaultValue);
            console.error('_default', _default);
         }
      }

      if (typeof _default === 'object') {
         if (props.getKeyValue) {
            defaultKeyValue = props.getKeyValue(_default);
         }
      } else {
         defaultKeyValue = _default;
      }
   }

   let name2 = props.name ? props.name : 'selectName';

   return (
      <div id={'select_' + name2}>
         {props.searchOnly && (
            <IconButton
               id={'icon'}
               style={{
                  fontSize: 10,
                  color: corDoIcone,
                  width: 38,
                  paddingTop: 4,
                  height: 28,
               }}
               onClick={showModal}
               icon={faCaretUp}
            />
         )}

         {!props.searchOnly && (
            <InputGroup style={{ flexWrap: 'nowrap' }}>
               <React.Fragment>
                  {!state.noDropDown && (
                     <FormControl
                        as='select'
                        className='select-dropdown-control'
                        ref={selectRef}
                        name={props.name ? props.name : 'selectName'}
                        id={name}
                        title={defaultDescription}
                        style={{
                           height: props.height ? props.height : 38,
                           padding: 1,
                           backgroundColor: props.color,
                           cursor: props.cursor ? props.cursor : 'default',
                           outline: 'none',
                           borderColor: '#ced4da',
                           fontSize: props.fontSize ? props.fontSize : null,
                        }}
                        defaultValue={defaultKeyValue}
                        onChange={(e) => {
                           let options = props.options.filter((i) => {
                              let value = props.getKeyValue ? props.getKeyValue(i) : null;
                              return (value || (value === 0 && props.acceptZero)) &&
                                 value.toString() === e.target.value
                                 ? true
                                 : false;
                           });
                           let option = options[0];
                           onSelect(option);
                        }}
                        disabled={props.disabled}
                        readOnly={props.readOnly}
                     >
                        {[
                           (props.allowEmpty === undefined || props.allowEmpty === null) && (
                              <option key={-1} value=''>
                                 {props.nullText !== null && props.nullText !== undefined
                                    ? props.nullText
                                    : 'selecione...'}
                              </option>
                           ),
                           getOptions(props.options).map((item, index) => {
                              return (
                                 <option
                                    key={index}
                                    value={props.getKeyValue ? props.getKeyValue(item) : null}
                                 >
                                    {props.getDescription ? props.getDescription(item) : null}
                                 </option>
                              );
                           }),
                        ]}
                     </FormControl>
                  )}

                  {state.noDropDown && (
                     <form
                        onSubmit={(event) => {
                           event.preventDefault();
                           setState(prev => ({ ...prev, inserindo: true }));
                        }}
                        action='/'
                        name={'formSearchText_' + name2}
                        id={'formSearchText_' + name2}
                        style={{ width: '100%' }}
                     >
                        <FormControl
                           type='text'
                           className='select-text-control'
                           ref={inputRef}
                           name={props.name ? props.name : 'selectName'}
                           id={name}
                           defaultValue={defaultDescription}
                           onChange={(e) => {
                              setState(prev => ({ ...prev, searchText: e.target.value }));
                              changedRef.current = true;
                              clearTimeout(changeTimerRef.current);
                              changeTimerRef.current = setTimeout(() => {
                                 filter(e.target.value);
                              }, 100);
                           }}
                           title={defaultDescription}
                           style={{
                              backgroundColor:
                                 (props.readOnly || state.oneItemSelected) && getReadOnlyColor()
                                    ? getReadOnlyColor()
                                    : props.color,
                              borderTopRightRadius: props.readOnly ? null : 0,
                              borderBottomRightRadius: props.readOnly ? null : 0,
                              outline: 'none',
                              boxShadow: 'none',
                              borderColor: '#ced4da',
                              fontSize: props.fontSize ? props.fontSize : null,
                           }}
                           readOnly={
                              props.readOnly || state.oneItemSelected || props.disableTextEdit
                                 ? true
                                 : false
                           }
                           placeholder={props.placeholder}
                           onInput={inputToUpper}
                           onBlur={() => {
                              setTimeout(() => {
                                 setState(prev => ({ ...prev, previewOptions: null }));
                              }, 1500);
                           }}
                           onKeyUp={(e) => {
                              if (e.keyCode === 40) {
                                 setFocus(1);
                              }
                              if (e.keyCode === 38) {
                                 setFocus(-1);
                              }
                           }}
                        />
                     </form>
                  )}

                  {props.formularioPadrao && !props.readOnly && (
                     <div className='hide-when-readonly' style={{ cursor: 'pointer' }}>
                        {!state.oneItemSelected && (
                           <InputGroup.Text
                              style={{ maxWidth: 48 }}
                              onClick={showModal}
                           >
                              <div style={{ height: 24, width: 30, display: 'table-cell' }}>
                                 <IconButton
                                    style={{
                                       fontSize: 22,
                                       paddingTop: 3,
                                       color: corDoIcone,
                                    }}
                                    icon={state.noDropDown ? faSearch : faSearch}
                                 />
                              </div>
                           </InputGroup.Text>
                        )}
                        {state.oneItemSelected && (
                           <InputGroup.Text style={{ cursor: 'pointer' }} onClick={clear}>
                              <React.Fragment>
                                 <IconButton
                                    style={{
                                       fontSize: 22,
                                       paddingTop: 3,
                                       color: corDoIcone,
                                    }}
                                    icon={faTimesCircle}
                                 />
                              </React.Fragment>
                           </InputGroup.Text>
                        )}
                     </div>
                  )}
               </React.Fragment>
            </InputGroup>
         )}

         {state.noDropDown && state.previewOptions && state.previewOptions.length > 0 && (
            <div style={{ position: 'relative', zIndex: 999 }}>
               <DropDownStyled>
                  {state.previewOptions &&
                     state.previewOptions.map((item, index) => {
                        return (
                           <div
                              key={index}
                              tabIndex={1000 + index}
                              id={name + '__' + (index + 1)}
                              style={{
                                 width: '100%',
                                 border: '1px solid lightgray',
                                 padding: 8,
                                 cursor: 'pointer',
                                 color: props.dropDownTextColor ? props.dropDownTextColor : '#555',
                              }}
                              onClick={() => {
                                 aoSelecionar(item);
                                 setState(prev => ({ ...prev, previewOptions: null }));
                              }}
                              onKeyUp={(e) => {
                                 if (e.keyCode === 40) {
                                    setFocus(1);
                                 }
                                 if (e.keyCode === 38) {
                                    setFocus(-1);
                                 }
                                 if (e.keyCode === 13) {
                                    aoSelecionar(item);
                                    setState(prev => ({ ...prev, previewOptions: null }));
                                    let component = document.getElementById(name);
                                    if (component) {
                                       component.focus();
                                    }
                                 }
                                 if (e.keyCode === 27) {
                                    aoSelecionar(null);
                                    setState(prev => ({ ...prev, previewOptions: null }));
                                    let component = document.getElementById(name);
                                    if (component) {
                                       component.focus();
                                    }
                                 }
                              }}
                           >
                              <Text>{props.getDescription(item)}</Text>
                           </div>
                        );
                     })}
               </DropDownStyled>
            </div>
         )}

         {state.inserindo && (
            <Modal
               show={state.inserindo}
               scrollable={true}
               size={'lg'}
               onHide={() => {}}
               onKeyDown={(e) => {
                  if (e.keyCode === 27) setState(prev => ({ ...prev, inserindo: false }));
               }}
               dialogClassName='h-100'
            >
               <Modal.Body
                  style={{
                     overflow: 'hidden',
                     display: 'flex',
                     position: 'relative',
                     fontSize: 13,
                     padding: '0 0 0 0',
                     maxHeight: '100%',
                  }}
               >
                  {props.formularioPadrao({ aoSelecionar, aoCancelar, getSearchText })}
               </Modal.Body>
            </Modal>
         )}
      </div>
   );
}

const DropDownStyled = styled.div`
   width: ${(props) => props.width};
   position: absolute;
   top: -3px;
   left: 0px;
   width: 100%;
   background-color: white;
   z-index: 1;

   div:last-child {
      border-bottom-left-radius: 5px;
      border-bottom-right-radius: 5px;
   }
`;
