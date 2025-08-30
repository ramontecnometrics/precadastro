import React, { useState, useEffect } from 'react';
import { LayoutParams } from '../../config/LayoutParams';
import '../contents/css/cabecalho-tabela-formulario-padrao.css';
import '../contents/css/tabela-formulario-padrao.css';
import Button from '../Button';

export default function ListaPadraoOffline(props) {
   const [state, setState] = useState({
      itemSelecionado: null,
      itens: null,
      ordenacao: props.ordenacaoPadrao,
      tamanhoDaPagina: null
   });

   // Bind methods from props or use defaults
   const renderizarFiltros = props.renderizarFiltros || defaultRenderizarFiltros;
   const getTitulosDaTabela = props.getTitulosDaTabela || defaultGetTitulosDaTabela;
   const getDadosDaTabela = props.getDadosDaTabela || defaultGetDadosDaTabela;

   useEffect(() => {
      try {
         if (props.ref) {
            props.ref({ filtrar, cancelarClick });
         }

         let currentState = state;

         currentState.itemSelecionado = null;
         currentState.itens = null;
         currentState.ordenacao = props.ordenacaoPadrao;
         currentState.tamanhoDaPagina = null;

         setState(currentState);
         
         if (!props.iniciarVazio) {
            filtrar();
         }
      } catch (e) {
         console.error(e);
      }
   }, []);

   // Default methods
   function defaultRenderizarFiltros(sender) {
      // * OBRIGATÓRIO
      // Implemente esse método para desenhar os campos de filtro na tela
   }

   function defaultGetTitulosDaTabela() {
      // * OBRIGATÓRIO
      // Implemente esse método no para retornar um array de string com os títulos para a tabela
      return [];
   }

   function defaultGetDadosDaTabela(item, index) {
      // * OBRIGATÓRIO
      // Implemente esse método no para retornar um array de valores a ser usado na linha da tabela.
      // Este método é chamado passando registro por registro.
      return [];
   }

   const formConsultaSubmit = (event) => {
      event.preventDefault();
      filtrar();
   };

   const isMobile = () => {
      return window.screen.width <= 600;
   };

   const getNavegador = () => {
      return (
         <div style={{ width: '100%', display: 'flex', justifyContent: 'flex-end', padding: '10px 12px 10px 10px' }}>
            <div style={{ width: 150, textAlign: 'left' }}>
               {!props.esconderBotaoFechar && (
                  <Button
                     variant='secondary'
                     onClick={cancelarClick}
                     style={{ width: 150 }}
                     text={'Fechar'}
                  />
               )}
            </div>
         </div>
      );
   };

   const onRowClick = (item, index) => {
      if (props.select) {
         props.select.aoSelecionar(item);
      }
   };

   const handleClick = (item, index) => {
      if (props.onRowClick) {
         props.onRowClick(item, index);
      }
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
            setState(prev => ({
               ...prev,
               itens: props.getItens(),
            }));
            resolve();
         } catch (e) {
            reject(e);
         }
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
            <div style={{ paddingTop: 0, paddingLeft: 10, paddingRight: 10, height: 45, display: 'flex' }}>
               <div
                  style={{
                     display: 'table-cell',
                     width: '0',
                     overflowX: 'visible',
                     fontSize: 30,
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
         <form
            style={{
               paddingTop: 8,
               paddingLeft: 10,
               paddingRight: 14,
            }}
            onSubmit={formConsultaSubmit}
            action='/'
            name='formConsulta'
            id='formConsulta'
         >
            {renderizarFiltros({ filtrar, cancelarClick })}
         </form>
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
                        return (
                           <td key={index} className={item.className} style={{ width: item.width }}>
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
      let itens = state.itens;

      if (isMobile) {
         result =
            !itens || (itens && itens.length === 0) ? (
               <table className='tabela-formulario-padrao'>
                  <tbody>
                     <tr>
                        <td style={{ width: '100%', textAlign: 'center' }}>
                           <Text>{'Nenhum registro encontrado'}</Text>
                        </td>
                     </tr>
                  </tbody>
               </table>
            ) : (
               itens.map((item, rowIndex) => {
                  return !item ? null : (
                     <div
                        key={rowIndex}
                        className={props.select ? 'noselect' : null}
                        style={{
                           cursor: props.select || props.onRowClick ? 'pointer' : 'default',
                           border: '1px solid #999',
                           borderRadius: 5,
                           marginBottom: 7,
                           marginRight: 10,
                           padding: 3,
                        }}
                        onClick={() => handleClick(item, rowIndex)}
                     >
                        {getDadosDaTabela(item, rowIndex).map((dado, campoIndex) => {
                           return (
                              <div key={campoIndex} style={{ display: 'flex', flexDirection: 'row' }}>
                                 <div style={{ display: 'table-cell' }}>
                                    <strong>{textoDosTitulos[campoIndex]}:&nbsp;</strong>
                                 </div>
                                 <div style={{ display: 'table-cell' }}>
                                    <Text style={{ wordWrap: 'anywhere' }}>{dado}</Text>
                                 </div>
                              </div>
                           );
                        })}
                     </div>
                  );
               })
            );
      } else {
         result =
            itens != null && itens && itens.length === 0 ? (
               <table className='tabela-formulario-padrao'>
                  <tbody>
                     <tr>
                        <td style={{ width: '100%', textAlign: 'center' }}>
                           <Text>{'Nenhum registro encontrado'}</Text>
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
                                    cursor: props.select || props.onRowClick ? 'pointer' : 'default',
                                    backgroundColor:
                                       props.highlightIndex === rowIndex
                                          ? LayoutParams.colors.tableItemhHighlightBackgroundColor
                                          : null,
                                 }}
                                 onClick={() => handleClick(item, rowIndex)}
                              >
                                 {[
                                    getDadosDaTabela(item).map((dado, campoIndex) => {
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
         !props.esconderConsulta && (
            <div style={{ display: 'flex', flexDirection: 'row', paddingBottom: 10, paddingLeft: 10 }}>
               <div style={{ display: 'table-cell' }}>
                  <Button
                     style={{ width: 200 }}
                     text={'Pesquisar'}
                     inProgressText={'pesquisando...'}
                     onClickAsync={filtrar}
                  />
               </div>
            </div>
         )
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
