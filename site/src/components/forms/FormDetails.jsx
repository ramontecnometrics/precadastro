import React, { useEffect, useState } from 'react';
import IconButton from '../IconButton';
import { faTrashAlt, faEdit } from '@fortawesome/free-regular-svg-icons';
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import '../../contents/css/sub-cadastro.css';
import Button from '../Button';
import Line from '../Line';
import { showConfirm } from '../Messages';
import Filler from '../Filler';
import { Modal } from 'react-bootstrap';
import { LayoutParams } from '../../config/LayoutParams';
import Text from '../Text';

export default function FormDetails(props) {
   const [state, setState] = useState({
      inserindo: false,
      alterando: false,
      exibirTitulos: props.exibirTitulos === false ? false : true,
      exibirRodape: props.exibirRodape === true ? true : false,
      indiceEmEdicao: null,
   });

   useEffect(() => {
      if (props.autoFocus && props.autoFocus.current && (state.inserindo || state.alterando)) {
         setTimeout(() => {
            props.autoFocus.current.focus();
         }, 100);
      }
   }, [state.inserindo, state.alterando]);

   const renderizarAcoes = (index) => {
      return props.alterar ||
         (props.excluir && ((props.podeExcluir && props.podeExcluir(index)) || !props.podeExcluir)) ||
         props.acoes ? (
         <td key='acoes' className='acoes'>
            <div style={{ display: 'flex', justifyContent: 'center' }}>
               {props.alterar &&
                  (!props.gradeSempreVisivel ||
                     (props.gradeSempreVisivel && !(state.inserindo || state.alterando))) && (
                     <div style={{ display: 'table-cell', margin: 'auto' }}>
                        <AlterFormDetailItemButtom onClick={() => selecionarParaAlteracao(index)} title={'Alterar'} />
                     </div>
                  )}
               {props.excluir && ((props.podeExcluir && props.podeExcluir(index)) || !props.podeExcluir) && (
                  <div style={{ display: 'table-cell', margin: 'auto' }}>
                     <DeleteFormDetailItemButton
                        onClick={() => {
                           props.excluir(index);
                        }}
                        title={'Excluir'}
                     />
                  </div>
               )}
               {props.acoes && props.acoes({}, index)}
            </div>
         </td>
      ) : null;
   };

   const novo = async () => {
      if (props.formularioPadrao) {
         setState((prev) => ({
            ...prev,
            inserindo: true,
            alterando: false,
            indiceEmEdicao: null,
         }));
      } else if (props.novo) {
         await props.novo();
         setState((prev) => ({
            ...prev,
            inserindo: true,
            alterando: false,
            indiceEmEdicao: null,
         }));
      } else {
         setState((prev) => ({
            ...prev,
            inserindo: true,
            alterando: false,
            indiceEmEdicao: null,
         }));
      }
   };

   const cancelar = async () => {
      if (props.cancelar) {
         props.cancelar();
         setState((prev) => ({
            ...prev,
            inserindo: false,
            alterando: false,
            indiceEmEdicao: null,
         }));
      } else {
         setState((prev) => ({
            ...prev,
            inserindo: false,
            alterando: false,
            indiceEmEdicao: null,
         }));
      }
   };

   const selecionarParaAlteracao = async (index) => {
      if (props.selecionarParaAlteracao) {
         await props.selecionarParaAlteracao(index);
         setState((prev) => ({ ...prev, alterando: true, inserindo: false, indiceEmEdicao: index }));
      }
   };

   const salvar = () => {
      return new Promise((resolve, reject) => {
         state.inserindo
            ? props
                 .inserir()
                 .then(() => {
                    setState((prev) => ({
                       ...prev,
                       inserindo: false,
                       alterando: false,
                    }));
                    resolve();
                    if (props.aposSalvar) {
                       props.aposSalvar({}, true, false);
                    }
                 })
                 .catch((e) => {
                    console.error(e);
                    if (reject) {
                       reject(e);
                    }
                 })
            : props
                 .alterar(state.indiceEmEdicao)
                 .then(() => {
                    setState((prev) => ({
                       ...prev,
                       inserindo: false,
                       alterando: false,
                    }));
                    resolve();
                    if (props.aposSalvar) {
                       props.aposSalvar({}, false, true);
                    }
                 })
                 .catch((e) => {
                    console.error(e);
                    if (reject) {
                       reject(e);
                    }
                 });
      });
   };

   const aoCancelar = () => {
      setState((prev) => ({ ...prev, inserindo: false }));
   };

   const aoSelecionar = (item) => {
      console.log('aoSelecionar');
      if (props.aoSelecionar) {
         props.aoSelecionar(item).then(() => {
            setState((prev) => ({
               ...prev,
               inserindo: false,
               alterando: false,
            }));
            if (props.aposSalvar) {
               props.aposSalvar({}, true, false);
            }
         });
      }
   };

   const colunas = props.colunas && props.colunas();
   const tamanhos = colunas && colunas.map((i) => i.width);
   const classes = colunas && colunas.map((i) => i.className);

   return (
      <div className='sub-cadastro'>
         <table
            className={'table-hover ' + (props.itens && props.itens.length ? '' : 'empty-table')}
            style={state.inserindo || state.alterando ? props.formStyle : props.tableStyle}
         >
            <thead>
               <tr>
                  <th colSpan={10} style={{ width: '100%' }}>
                     {((!state.inserindo && !state.alterando) || props.modal || props.formularioPadrao) && (
                        <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
                           <div style={{ width: '100%', display: 'table-cell' }}>
                              <div style={{ fontWeight: '600', paddingTop: 6 }}>
                                 <Text>{props.titulo}</Text>
                              </div>
                           </div>
                           <div
                              className='hide-when-readonly'
                              style={{ width: 30, display: 'table-cell', textAlign: 'right' }}
                           >
                              {props.podeInserir !== false && (props.novo || props.formularioPadrao) && (
                                 <IconButton
                                    title={'Novo'}
                                    className='sub-cadastro-btn-novo'
                                    style={{
                                       fontSize: 22,
                                       paddingTop: 2,
                                       paddingRight: 5,
                                       color: LayoutParams.colors.corDosBotoesDoFormularioPadrao,
                                    }}
                                    cursor='pointer'
                                    icon={faPlusCircle}
                                    onClick={novo}
                                 />
                              )}
                           </div>
                        </div>
                     )}

                     {(state.inserindo || state.alterando) &&
                        props.formulario &&
                        !props.formularioPadrao &&
                        !props.modal && (
                           <React.Fragment>
                              <div style={{ fontWeight: '600', paddingTop: 6 }}>
                                 <Text>{props.titulo}</Text>
                              </div>
                              <Line marginTop={5} marginBottom={6} />
                              <div style={{ fontWeight: 'normal', paddingLeft: 5, paddingRight: 5 }}>
                                 {props.formulario({ aoCancelar, aoSelecionar })}
                              </div>
                              <Line marginTop={5} marginBottom={6} />
                              <div
                                 style={{
                                    display: 'flex',
                                    flexDirection: 'row',
                                    justifyContent: 'flex-end',
                                 }}
                              >
                                 <div style={{ display: 'table-cell' }}>
                                    <Button
                                       text={props.tituloDoBotaoCancelar ? props.tituloDoBotaoCancelar : 'Cancelar'}
                                       onClick={cancelar}
                                       style={{ fontSize: 16, height: 33, minWidth: 80 }}
                                    />
                                 </div>
                                 <div style={{ display: 'table-cell' }}>
                                    <Button
                                       text={state.alterando ? 'OK' : 'Inserir'}
                                       style={{ fontSize: 16, height: 33, minWidth: 80 }}
                                       onClick={salvar}
                                    />
                                 </div>
                              </div>
                           </React.Fragment>
                        )}

                     {(state.inserindo || state.alterando) &&
                        props.formulario &&
                        !props.formularioPadrao &&
                        props.modal && (
                           <Modal
                              show={state.inserindo || state.alterando}
                              scrollable={true}
                              onHide={() => {}}
                              onKeyDown={(e) => {
                                 if (e.keyCode === 27) cancelar();
                              }}
                              size='lg'
                              aria-labelledby='contained-modal-title-vcenter'
                              centered
                              stype={{ padding: 0 }}
                           >
                              <div
                                 style={{
                                    backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
                                    color: LayoutParams.colors.corSecundaria,
                                    height: 42,
                                    fontSize: 24,
                                    padding: '3px 0 0 5px',
                                 }}
                              >
                                 <Text>{props.titulo}</Text>
                              </div>

                              <Modal.Body
                                 style={{
                                    padding: '0 15px 15px 15px',
                                    fontSize: 13,
                                 }}
                              >
                                 <div>
                                    <Filler height={5} />
                                    <div style={{ fontWeight: 'normal' }}>
                                       {props.formulario({ aoCancelar, aoSelecionar })}
                                    </div>
                                    <Line marginTop={5} marginBottom={6} />
                                    <div
                                       style={{
                                          display: 'flex',
                                          flexDirection: 'row',
                                          justifyContent: 'flex-end',
                                       }}
                                    >
                                       <div style={{ display: 'table-cell' }}>
                                          <Button
                                             text={
                                                props.tituloDoBotaoCancelar ? props.tituloDoBotaoCancelar : 'Cancelar'
                                             }
                                             onClick={cancelar}
                                             style={{ fontSize: 16, height: 33 }}
                                          />
                                       </div>
                                       <div style={{ display: 'table-cell' }}>
                                          <Button
                                             text={state.alterando ? 'OK' : 'Inserir'}
                                             inProgressText={'salvando...'}
                                             style={{ fontSize: 16, height: 33, minWidth: 80 }}
                                             onClickAsync={salvar}
                                          />
                                       </div>
                                    </div>
                                 </div>
                              </Modal.Body>
                           </Modal>
                        )}

                     {state.inserindo && props.formularioPadrao && (
                        <Modal
                           show={state.inserindo}
                           scrollable={true}
                           onHide={() => {}}
                           onKeyDown={(e) => {
                              if (e.keyCode === 27) cancelar();
                           }}
                           size='lg'
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
                              {props.formularioPadrao({ aoCancelar, aoSelecionar })}
                           </Modal.Body>
                        </Modal>
                     )}
                  </th>
               </tr>

               {((!state.inserindo && !state.alterando) || props.modal || props.gradeSempreVisivel) &&
                  state.exibirTitulos &&
                  props.itens &&
                  props.itens.length > 0 &&
                  colunas && (
                     <tr>
                        <React.Fragment>
                           {colunas.map((item, index) => {
                              return (
                                 <th key={index} className={item.className} style={{ width: item.width }}>
                                    {item.titulo}
                                 </th>
                              );
                           })}
                        </React.Fragment>
                        <React.Fragment>
                           {(props.alterar || props.excluir || props.acoes) && (
                              <th key='acoes' className='acoes'>
                                 {props.tituloDasAcoes ? props.tituloDasAcoes() : 'Ações'}
                              </th>
                           )}
                        </React.Fragment>
                     </tr>
                  )}
            </thead>

            {((!state.inserindo && !state.alterando) ||
               props.modal ||
               props.formularioPadrao ||
               props.gradeSempreVisivel) && (
               <React.Fragment>
                  {!props.itens || props.itens.length === 0 ? (
                     <tbody>
                        <tr>
                           <td style={{ width: '100%', textAlign: 'center', color: '#999' }}>
                              <Text>{'Nenhum registro encontrado'}</Text>
                           </td>
                        </tr>
                     </tbody>
                  ) : (
                     <tbody>
                        {props.itens.map((item, rowIndex) => {
                           return !item ? null : (
                              <tr
                                 key={rowIndex}
                                 className={props.select ? 'noselect' : null}
                                 style={{
                                    cursor: props.select ? 'pointer' : 'default',
                                    fontWeight: 'normal',
                                 }}
                              >
                                 <React.Fragment>
                                    {props.linha(item).map((dado, campoIndex) => {
                                       return (
                                          <td
                                             key={campoIndex}
                                             className={classes && classes[campoIndex]}
                                             style={{ width: tamanhos && tamanhos[campoIndex] }}
                                          >
                                             {dado}
                                          </td>
                                       );
                                    })}
                                 </React.Fragment>
                                 <React.Fragment>{renderizarAcoes(rowIndex)}</React.Fragment>
                              </tr>
                           );
                        })}
                     </tbody>
                  )}
               </React.Fragment>
            )}
            {!state.inserindo && !state.alterando && state.exibirRodape && props.itens && props.itens.length > 0 ? (
               <tfoot>{props.rodape()}</tfoot>
            ) : null}
         </table>
      </div>
   );
}

function AlterFormDetailItemButtom(props) {
   return (
      <IconButton
         title={'Alterar'}
         style={{
            color: LayoutParams.colors.corDosBotoesDoFormularioPadrao,
         }}
         cursor='pointer'
         className='sub-cadastro-btn-alterar'
         icon={faEdit}
         onClick={props.onClick}
      />
   );
}

function DeleteFormDetailItemButton(props) {
   return (
      <IconButton
         title={'Excluir'}
         style={{
            color: LayoutParams.colors.corDosBotoesDoFormularioPadrao,
         }}
         cursor='pointer'
         className='sub-cadastro-btn-alterar'
         icon={faTrashAlt}
         onClick={() => {
            if (props.onClick) {
               showConfirm('Deseja realmente excluir este registro?', () => {
                  props.onClick();
               });
            }
         }}
      />
   );
}
