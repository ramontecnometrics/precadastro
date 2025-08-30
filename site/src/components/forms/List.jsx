import React, { useState, useEffect, useRef } from 'react';
import { LayoutParams } from '../../config/LayoutParams';
import '../contents/css/cabecalho-tabela-formulario-padrao.css';
import '../contents/css/tabela-formulario-padrao.css';
import Button from '../Button';
import { buildQueryString } from '../../utils/Functions';
import { ButtonGroup } from 'react-bootstrap';
import { faAngleDoubleDown, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import api from '../../utils/Api';

let mostrarCodigo = false;

export default function ListaPadrao(props) {
   const [state, setState] = useState({
      itemSelecionado: null,
      itens: null,
      filtro: {},
      ordenacao: props.ordenacaoPadrao,
      tamanhoDaPagina: Math.trunc(800 / 36.8),
      quantidadeDeDados: null,
      quantidadeTotalDeDados: null,
      podeAvancar: false,
      podeCarregarTodos: false,
      vazio: true,
      carregando: false
   });

   const setFormState = props.setFormState || setState;
   const getFormState = props.getFormState || (() => state);

   // Bind methods from props or use defaults
   const renderizarFiltros = props.renderizarFiltros || defaultRenderizarFiltros;
   const getFiltro = props.getFiltro || defaultGetFiltro;
   const getTitulosDaTabela = props.getTitulosDaTabela || defaultGetTitulosDaTabela;
   const getDadosDaTabela = props.getDadosDaTabela || defaultGetDadosDaTabela;

   useEffect(() => {
      try {
         if (props.ref) {
            props.ref({ filtrar, cancelarClick });
         }

         let clientHeight = 800;

         setFormState(
            {
               itemSelecionado: null,
               itens: null,
               filtro: {},
               ordenacao: props.ordenacaoPadrao,
               tamanhoDaPagina: Math.trunc(clientHeight / 36.8),
            },
            () => {
               if (!props.iniciarVazio) {
                  filtrar();
               }
            }
         );
      } catch (e) {
         console.error(e);
      }
   }, []);

   // Default methods
   function defaultRenderizarFiltros(sender) {
      // * OBRIGATÓRIO
      // Implemente esse método para desenhar os campos de filtro na tela
   }

   function defaultGetFiltro(sender) {
      // * OBRIGATÓRIO
      // Implemente esse método para retornar o objeto a ser passado via POST para a url de pesquisa
      return Promise.resolve({});
   }

   function defaultGetTitulosDaTabela() {
      // * OBRIGATÓRIO
      // Implemente esse método no para retornar um array de string com os títulos para a tabela
      return [];
   }

   function defaultGetDadosDaTabela(item) {
      // * OBRIGATÓRIO
      // Implemente esse método no para retornar um array de valores a ser usado na linha da tabela.
      // Este método é chamado passando registro por registro.
      return [];
   }

   const isMobile = () => {
      return window.screen.width <= 600;
   };

   const getNavegador = () => {
      return (
         <div
            style={{
               width: '100%',
               display: 'flex',
               justifyContent: 'flex-end',
               padding: '10px 12px 10px 10px',
            }}
         >
            {!props.esconderBotaoFechar && (
               <div style={{ width: 150, textAlign: 'left' }}>
                  <Button
                     variant='secondary'
                     onClick={cancelarClick}
                     style={{ width: 150 }}
                     text={'Fechar'}
                  />
               </div>
            )}

            {getFormState().itens ? (
               <React.Fragment>
                  <div
                     style={{
                        width: '100%',
                        textAlign: 'left',
                        paddingTop: 10,
                        paddingLeft: 5,
                     }}
                  >
                     {getFormState().quantidadeTotalDeDados ? (
                        <div>
                           <Text>{"Mostrando"}&nbsp; </Text>
                           <Text>{getFormState().quantidadeDeDados}</Text>
                           <Text>&nbsp;{'de'}&nbsp;</Text>
                           <Text>{getFormState().quantidadeTotalDeDados}</Text>
                        </div>
                     ) : null}
                  </div>
                  <ButtonGroup className='mr-2' style={{ minWidth: 120 }}>
                     <Button
                        title={'Carregar mais...'}
                        text=''
                        icon={<FontAwesomeIcon icon={faAngleDown} />}
                        variant='secondary'
                        onClick={() => navegar(1)}
                        disabled={!getFormState().podeAvancar}
                        style={{
                           cursor: getFormState().podeAvancar ? 'pointer' : 'not-allowed',
                           padding: 0,
                        }}
                     />
                     <Button
                        title={'Carregar todos'}
                        text=''
                        icon={<FontAwesomeIcon icon={faAngleDoubleDown} />}
                        variant='secondary'
                        onClick={() => navegar(2)}
                        disabled={!getFormState().podeCarregarTodos}
                        style={{
                           cursor: getFormState().podeCarregarTodos ? 'pointer' : 'not-allowed',
                           padding: 0,
                        }}
                     />
                  </ButtonGroup>
               </React.Fragment>
            ) : null}
         </div>
      );
   };

   const cancelarClick = () => {
      return new Promise((resolve) => {
         if (props.select && props.select.aoCancelar) {
            props.select.aoCancelar();
         }
         if (props.aoCancelar) {
            props.aoCancelar();
         }
         if (props.aoFechar) {
            props.aoFechar();
         }
         resolve();
      });
   };

   const filtrar = () => {
      return new Promise((resolve, reject) => {
         try {
            setFormState(
               {
                  itens: [],
                  quantidadeDeDados: null,
                  quantidadeTotalDeDados: null,
               },
               () => {
                  getFiltro()
                     .then((filtro) => {
                        setFormState({ carregando: true, vazio: true });
                        let query = '';
                        if (filtro) {
                           if (props.estruturaPadrao) {
                              if (filtro && filtro.id) {
                                 query = '?id=' + filtro.id.toString;
                              } else {
                                 var orderBy = getFormState().ordenacao;
                                 query = buildQueryString(getFormState().tamanhoDaPagina, null, orderBy, filtro);
                              }
                           } else {
                              query = Object.keys(filtro)
                                 .map((key) => {
                                    var result = null;
                                    if (encodeURIComponent(filtro[key]) !== 'null') {
                                       result = `${encodeURIComponent(key)}=${encodeURIComponent(filtro[key])}`;
                                    }
                                    return result;
                                 })
                                 .join('&');
                              if (query) {
                                 query = '?' + query;
                              }
                           }
                        }
                        return api
                           .get(props.url + query)
                           .then((data) => {
                              let quantidadeDeDados = null;
                              let quantidadeTotalDeDados = null;
                              let podeAvancar = null;
                              let podeCarregarTodos = null;
                              let vazio = null;

                              if (props.estruturaPadrao) {
                                 quantidadeDeDados = data.pageSize;
                                 quantidadeTotalDeDados = data.count;
                                 podeAvancar = data.count >= getFormState().tamanhoDaPagina;
                                 podeCarregarTodos = data.count >= getFormState().tamanhoDaPagina;
                                 vazio = !data || data.count === 0;
                              } else {
                                 quantidadeDeDados = data && data.length;
                                 quantidadeTotalDeDados = data && data.length;
                                 podeAvancar = false;
                                 podeCarregarTodos = false;
                                 vazio = !data || data.length === 0;
                              }

                              setFormState(
                                 {
                                    itens: props.estruturaPadrao ? data.items : data,
                                    quantidadeDeDados: quantidadeDeDados,
                                    quantidadeTotalDeDados: quantidadeTotalDeDados,
                                    podeAvancar: podeAvancar,
                                    podeCarregarTodos: podeCarregarTodos,
                                    vazio: vazio,
                                    carregando: false,
                                 },
                                 resolve
                              );
                           })
                           .catch(reject);
                     })
                     .catch(reject);
               }
            );
         } catch (e) {
            reject(e);
         }
      });
   };

   const navegar = (opcao) => {
      let query = '';
      let currentState = getFormState();
      var orderBy = currentState.ordenacao;

      getFiltro().then((filtro) => {
         if (filtro && filtro.id) {
            query = '?id=' + filtro.id.toString;
         } else {
            query = buildQueryString(getFormState().tamanhoDaPagina, currentState.itens.length.toString(), orderBy, filtro);

            if (opcao === 1) {
               query = buildQueryString(
                  getFormState().tamanhoDaPagina,
                  currentState.itens.length.toString(),
                  orderBy,
                  filtro
               );
            } else {
               query = buildQueryString(null, null, orderBy, filtro);
               currentState.itens = [];
            }
         }

         setFormState(
            {
               quantidadeDeDados: null,
               quantidadeTotalDeDados: null,
            },
            () => {
               api
                  .getAll(props.url + query)
                  .then((data) => {
                     currentState.itens.push(...data.items);
                     setFormState({
                        itens: currentState.itens,
                        quantidadeDeDados: currentState.itens.length,
                        quantidadeTotalDeDados: data.count,
                        podeAvancar: data.count > currentState.itens.length,
                        podeCarregarTodos: data.count > currentState.itens.length,
                        vazio: !data || data.count === 0,
                     });
                  })
                  .catch((e) => console.error(e));
            }
         );
      });
   };

   const renderizarCodigo = (codigo) => {
      return (
         <td className='codigo'>
            <div>{codigo}</div>
         </td>
      );
   };

   const getTitulo = () => {
      return (
         <div
            style={{
               backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
               color: LayoutParams.colors.corSecundaria,
               borderRadius: 0,
               marginLeft: 0,
               marginRight: 0,
            }}
         >
            <div
               style={{
                  paddingTop: 0,
                  paddingLeft: 10,
                  paddingRight: 10,
                  height: 45,
                  display: 'flex',
               }}
            >
               <div
                  style={{
                     display: 'table-cell',
                     width: '0',
                     overflowX: 'visible',
                     fontSize: 22,
                     fontWeight: 500,
                     whiteSpace: 'nowrap',
                  }}
               >
                  <div>{props.titulo}</div>
               </div>
            </div>
         </div>
      );
   };

   const getFiltros = () => {
      return (
         <div
            style={{
               paddingTop: 8,
               paddingLeft: 10,
               paddingRight: 14,
            }}
         >
            {renderizarFiltros({ filtrar, cancelarClick })}
         </div>
      );
   };

   const getCabecalhos = () => {
      const isMobile = isMobile();

      var titulos = getTitulosDaTabela && getTitulosDaTabela();
      if (!titulos || isMobile) {
         return null;
      }
      return (
         <div className='div-cabecalho-tabela-formulario-padrao'>
            <table className='cabecalho-tabela-formulario-padrao'>
               <thead>
                  <tr>
                     {titulos.map((item, index) => {
                        if (!mostrarCodigo && index === 0) return null;
                        else
                           return (
                              <td
                                 key={index}
                                 className={item.className}
                                 style={{
                                    cursor: item.orderby && props.estruturaPadrao ? 'pointer' : 'default',
                                    width: item.width,
                                 }}
                                 onClick={() => {
                                    if (item.orderby && props.estruturaPadrao) {
                                       let ordenacao = item.orderby;
                                       if (ordenacao === getFormState().ordenacao) {
                                          ordenacao += ' desc';
                                       }
                                       setFormState({ ordenacao: ordenacao }, () => {
                                          filtrar();
                                       });
                                    }
                                 }}
                              >
                                 {item.titulo}
                              </td>
                           );
                     })}
                  </tr>
               </thead>
            </table>
         </div>
      );
   };

   const getLista = () => {
      const isMobile = isMobile();
      const titulos = getTitulosDaTabela && getTitulosDaTabela();
      const textoDosTitulos = titulos.map((i) => i.titulo);
      let result = null;

      if (!titulos) {
         return null;
      }

      const tamanhos = titulos.map((i) => i.width);
      const classes = titulos.map((i) => i.className);
      let itens = getFormState().itens;

      if (isMobile) {
         result = getFormState().vazio ? (
            <table className='tabela-formulario-padrao'>
               <tbody>
                  <tr>
                     <td style={{ width: '100%', textAlign: 'center', color: '#999' }}>
                        <Text>
                           {getFormState().carregando
                              ? 'Carregando...'
                              : 'Nenhum registro encontrado'}
                        </Text>
                     </td>
                  </tr>
               </tbody>
            </table>
         ) : (
            itens &&
            itens.map((item, rowIndex) => {
               return !item ? null : (
                  <div
                     key={rowIndex}
                     className={props.select ? 'noselect' : null}
                     style={{
                        cursor: props.select ? 'pointer' : 'default',
                        border: '1px solid #999',
                        borderRadius: 5,
                        marginBottom: 7,
                        marginRight: 10,
                        padding: 3,
                     }}
                  >
                     {getDadosDaTabela(item).map((dado, campoIndex) => {
                        return (
                           <div key={campoIndex} style={{ display: 'flex', flexDirection: 'row' }}>
                              <div style={{ display: 'table-cell' }}>
                                 <strong>{textoDosTitulos[campoIndex]}:&nbsp;</strong>
                              </div>
                              <div style={{ display: 'table-cell' }}>
                                 <span style={{ wordWrap: 'anywhere' }}>{dado}</Text>
                              </div>
                           </div>
                        );
                     })}
                  </div>
               );
            })
         );
      } else {
         result = getFormState().vazio ? (
            <table className='tabela-formulario-padrao'>
               <tbody>
                  <tr>
                     <td style={{ width: '100%', textAlign: 'center', color: '#999' }}>
                        <Text>
                           {getFormState().carregando
                              ? 'Carregando...'
                              : 'Nenhum registro encontrado'}
                        </Text>
                     </td>
                  </tr>
               </tbody>
            </table>
         ) : (
            <table className='tabela-formulario-padrao table-hover'>
               <tbody>
                  {itens != null &&
                     itens.map((item, rowIndex) => {
                        return !item ? null : (
                           <tr
                              key={rowIndex}
                              className={props.select ? 'noselect' : null}
                              style={{
                                 cursor: props.select ? 'pointer' : 'default',
                              }}
                           >
                              {[
                                 getDadosDaTabela(item).map((dado, campoIndex) => {
                                    if (!mostrarCodigo && campoIndex === 0) return null;
                                    else
                                       return (
                                          <td
                                             key={campoIndex}
                                             className={classes[campoIndex]}
                                             style={{ width: tamanhos[campoIndex] }}
                                          >
                                             {dado}
                                          </td>
                                       );
                                 }),
                              ]}
                           </tr>
                        );
                     })}
               </tbody>
            </table>
         );
      }
      return result;
   };

   const getAcoes = () => {
      return (
         <div
            style={{
               display: 'flex',
               flexDirection: 'row',
               paddingBottom: 10,
               paddingLeft: 10,
            }}
         >
            <div style={{ display: 'table-cell' }}>
               <Button
                  style={{ width: 200 }}
                  text={'Pesquisar'}
                  inProgressText={'pesquisando...'}
                  onClickAsync={filtrar}
               />
            </div>
         </div>
      );
   };

   return (
      <div
         id='listaPadrao'
         style={{
            display: 'flex',
            flexDirection: 'column',
            maxHeight: '100%',
            overflowX: 'hidden',
            width: '100%',
            maxWidth: '100%',
            height: '100%',
         }}
      >
         {!props.esconderTitulo && getTitulo()}
         {getFiltros()}
         {getAcoes()}
         {getCabecalhos()}
         <div className='div-tabela-formulario-padrao'>{getLista()}</div>
         {getNavegador()}
      </div>
   );
}
