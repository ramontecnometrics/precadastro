import React, { useEffect, useMemo, useRef, useImperativeHandle, forwardRef, useState } from 'react';
import { Navbar, Row, Col, ButtonGroup, Form as BootstrapForm, InputGroup, Modal } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTimesCircle, faSave } from '@fortawesome/free-regular-svg-icons';
import { faPlusCircle, faSearch, faAngleDown, faAngleDoubleDown } from '@fortawesome/free-solid-svg-icons';
import { LayoutParams } from '../../config/LayoutParams';
import { buildQueryString, isNumeric } from '../../utils/Functions';
import { showError, showInfo, showConfirm } from '../../components/Messages';
import Button from '../../components/Button';
import IconButton from '../../components/IconButton';
import api from '../../utils/Api';
import '../../contents/css/formulario-padrao.css';
import '../../contents/css/cabecalho-tabela-formulario-padrao.css';
import '../../contents/css/tabela-formulario-padrao.css';
import sessionManager from '../../SessionManager';
import BotaoAlterarItemDeCadastro from './BotaoAlterarItemDeCadastro';
import BotaoExcluirItemDeCadastro from './BotaoExcluirItemDeCadastro';
import Filler from '../Filler';
import BotaoVisualizarItemDeCadastro from './BotaoVisualizarItemDeCadastro';

let timer = 0;
let delay = 200;
let prevent = false;
let mostrarCodigo = false;

const Form = forwardRef(function Form(props, ref) {
   const {
      url,
      fastUrl,
      titulo,
      ordenacaoPadrao = 'Id',
      permissoes,
      getFiltro: getFiltroProp,
      filtroExtra,
      getTitulosDaTabela,
      getDadosDaTabela,
      renderizarFormulario: renderizarFormularioProp,
      getObjetoDeDados,
      getItemVazio: getItemVazioProp,
      antesDeInserir,
      aposInserir,
      antesDeEditar,
      aposEditar,
      antesDeSalvar,
      antesDeExcluir,
      aposSalvar,
      esconderFiltro,
      modal,
      select,
      form,
      apenasInserir,
      apenasAlterar,
      id,
      protected: isProtected,
   } = props;

   const [formState, setFormState] = useState({
      itemSelecionado: null,
      itens: [],
      vazio: false,
      consultou: false,
      navegando: true,
      incluindo: false,
      alterando: false,
      itemVazio: JSON.stringify(props.itemVazio || {}),
      filtro: {},
      botaoSalvarHabilitado: true,
      quantidadeDeDados: 0,
      quantidadeTotalDeDados: 0,
      podeAvancar: false,
      podeCarregarTodos: false,

      mostrarFormulario: true,
      ordenacao: ordenacaoPadrao,
      tamanhoDaPagina: 30,
      carregando: false,
      versaoAnterior: null,
   });

   // const podeConsultar = useMemo(
   //    () => (permissoes ? sessionManager.temAcessoARotina(permissoes[0]) : false),
   //    [permissoes]
   // );

   const podeIncluir = useMemo(
      () => (permissoes ? sessionManager.temAcessoARotina(permissoes[1]) : false),
      [permissoes]
   );
   const podeAlterar = useMemo(
      () => (permissoes ? sessionManager.temAcessoARotina(permissoes[2]) : false),
      [permissoes]
   );
   const podeExcluir = useMemo(
      () => (permissoes ? sessionManager.temAcessoARotina(permissoes[3]) : false),
      [permissoes]
   );

   const textoFiltroRef = useRef(null);
   const rootRef = useRef(null);

   const setMany = (patch) =>
      new Promise((resolve) => {
         setFormState((prev) => ({ ...prev, ...patch }));
         resolve();
      });

   const getItemVazio = () => {
      if (getItemVazioProp) {
         const r = getItemVazioProp();
         return r || {};
      }
      try {
         return JSON.parse(formState.itemVazio);
      } catch {
         return {};
      }
   };

   const buildFiltro = (f) => {
      console.log('Form.buildFiltro()');
      const result = filtroExtra ? filtroExtra() || {} : {};
      if (f && f.texto) {
         const texto = String(f.texto);
         if (texto[0] === '#') {
            if (!isNumeric(texto.substring(1))) {
               showError('Código inválido: "' + texto.substring(1) + '"');
               return result;
            }
            result.id = texto.substring(1);
         } else {
            result.searchable = texto;
         }
      }
      return getFiltroProp ? getFiltroProp(result) : result;
   };

   useEffect(() => {
      if (textoFiltroRef.current) {
         textoFiltroRef.current.focus();
         if (select?.getSearchText?.()) {
            const searchText = select.getSearchText();
            textoFiltroRef.current.selectionStart = searchText.length;
            textoFiltroRef.current.selectionEnd = searchText.length;
         }
      }
   }, [formState.filtro?.texto, select]);

   useEffect(() => {
      (async () => {
         let filtro = { ...formState.filtro };
         if (select?.getSearchText?.()) {
            filtro.texto = select.getSearchText();
         }
         await setMany({ filtro, navegando: true });

         if (apenasInserir) {
            inserirClick();
            return;
         }
         if (apenasAlterar && id != null) {
            try {
               const data = await api.getAll(`${url}?id=${String(id)}`);
               const item = data?.items?.[0];
               if (item) {
                  editarClick(item);
               } else {
                  showError('Registro não localizado para alteração');
               }
            } catch (e) {}
            return;
         }

         filtrar();
      })();
      // eslint-disable-next-line react-hooks/exhaustive-deps
   }, []);

   useEffect(() => {
      if (formState.incluindo || formState.alterando) {
         setTimeout(() => {            
            if (props.autoFocus && props.autoFocus.current) {               
               props.autoFocus.current.focus();
            }
         }, 100);
      } else if (formState.navegando && textoFiltroRef.current) {
         setTimeout(() => {
            console.log('focus');
            textoFiltroRef.current.focus();
         }, 100);
      }
   }, [formState.incluindo, formState.alterando, formState.navegando]);

   const isMobile = () => {
      return window.screen.width <= 600;
   };

   const limparFiltro = async () => {
      console.log('Form.limparFiltro()');
      const filtro = { ...(formState.filtro || {}) };
      filtro.texto = null;
      await setMany({ filtro });
      if (textoFiltroRef.current) {
         textoFiltroRef.current.value = '';
      }
      filtrar();
   };

   const filtrar = async () => {
      console.log('Form.filtrar()');
      await setMany({
         itens: [],
         quantidadeDeDados: null,
         quantidadeTotalDeDados: null,
         vazio: true,
         carregando: true,
      });

      let query = '';
      const filtro = formState.filtro;

      if (filtro && filtro.id) {
         query = `?id=${String(filtro.id)}`;
      } else {
         const orderBy = formState.ordenacao || ordenacaoPadrao;
         query = buildQueryString(formState.tamanhoDaPagina, null, orderBy, buildFiltro(filtro));
      }

      try {
         const data = await api.getAll((fastUrl || url) + query);
         await setMany({
            itens: data.items || [],
            quantidadeDeDados: data.pageSize || 0,
            quantidadeTotalDeDados: data.count || 0,
            podeAvancar:
               (data.count || 0) > (data.items?.length || 0) && (data.items?.length || 0) >= formState.tamanhoDaPagina,
            podeCarregarTodos: (data.count || 0) > (data.items?.length || 0),
            vazio: !data || data.count === 0,
            carregando: false,
            consultou: true,
         });
      } catch (e) {
         await setMany({ carregando: false });
      }
   };

   const navegar = async (opcao) => {
      console.log('Form.navegar()');
      let query = '';
      const orderBy = formState.ordenacao || ordenacaoPadrao;
      const filtro = formState.filtro;

      if (filtro && filtro.id) {
         query = `?id=${String(filtro.id)}`;
      } else {
         if (opcao === 1) {
            query = buildQueryString(
               formState.tamanhoDaPagina,
               String(formState.itens.length),
               orderBy,
               buildFiltro(filtro)
            );
         } else {
            query = buildQueryString(null, null, orderBy, buildFiltro(filtro));
            await setMany({ itens: [] });
         }
      }

      try {
         const data = await api.getAll((fastUrl || url) + query);
         const novos = data.items || [];
         const lista = opcao === 1 ? [...formState.itens, ...novos] : [...novos];

         await setMany({
            itens: lista,
            quantidadeDeDados: lista.length,
            quantidadeTotalDeDados: data.count || 0,
            podeAvancar: (data.count || 0) > lista.length,
            podeCarregarTodos: (data.count || 0) > lista.length,
            vazio: !data || data.count === 0,
         });
      } catch (e) {}
   };

   const inserirClick = async () => {
      console.log('Form.inserirClick()');
      const run = async () => {
         const item = getItemVazio();
         await setMany({ itemSelecionado: item });
         await setMany({ navegando: false, alterando: false, incluindo: true });
         if (aposInserir) {
            await aposInserir({ ...formState, itemSelecionado: item }, setFormState);
         }
      };
      if (antesDeInserir) {
         await Promise.resolve(antesDeInserir(formState, setFormState)).then(run);
      } else {
         await run();
      }
   };

   const getItem = async (idToFetch) => {
      const data = await api.getAll(`${url}?id=${String(idToFetch)}`);
      if (data?.items?.length > 1) {
         showError('Mais de um item encontrado para o filtro aplicado');
         throw new Error('Mais de um item encontrado para o filtro aplicado');
      }
      return data?.items?.[0] || null;
   };

   const editarClick = async (clickedItem) => {
      console.log('Form.editarClick()');
      const result = await getItem(clickedItem.id);
      if (!result) {
         showError('Registro não localizado para alteração');
         return;
      }
      const item = JSON.parse(JSON.stringify(result));

      const run = async () => {
         await setMany({ itemSelecionado: item, versaoAnterior: item });
         await setMany({ navegando: false, alterando: true, incluindo: false });
         if (aposEditar) {
            await aposEditar(formState, setFormState);
         }
      };
      if (antesDeEditar) {
         await Promise.resolve(antesDeEditar(formState, setFormState)).then(run);
      } else {
         await run();
      }
   };

   const cancelarClick = async () => {
      console.log('Form.cancelarClick()');
      await setMany({
         navegando: select || form ? false : true,
         alterando: false,
         incluindo: false,
         itemSelecionado: getItemVazio(),
      });

      if (select?.aoCancelar) select.aoCancelar();
      if (modal && props.aoCancelar) props.aoCancelar();
      if (form?.aoCancelar) form.aoCancelar();
   };

   const excluir = async (item) => {
      console.log('Form.excluir()');
      await api.delete(`${url}?id=${item.id}`);
      await showInfo('Excluído com sucesso!');
      await setMany({
         navegando: true,
         alterando: false,
         incluindo: false,
         itens: [],
         itemSelecionado: getItemVazio(),
      });
      filtrar();
   };

   const excluirClick = async (item) => {
      console.log('Form.excluirClick()');
      const run = () => showConfirm('Deseja realmente excluir este registro?', () => excluir(item));
      if (antesDeExcluir) await Promise.resolve(antesDeExcluir(formState, setFormState)).then(run);
      else run();
   };

   const salvar = async () => {
      console.log('Form.salvar()');
      if (antesDeSalvar) {
         await Promise.resolve(antesDeSalvar(formState, setFormState));
      }
      await setMany({ botaoSalvarHabilitado: false });

      try {
         const input = await Promise.resolve(getObjetoDeDados(formState, setFormState));
         await setMany({ botaoSalvarHabilitado: true });

         if (formState.incluindo) {
            const postFn = isProtected ? api.protectedPost.bind(api) : api.post.bind(api);
            const idGerado = await postFn(url, input);

            if (aposSalvar) await Promise.resolve(aposSalvar(formState, setFormState));

            if ((select && select.aoSelecionar) || (modal && props.aoSelecionar)) {
               const result = await api.getAll(`${url}?id=${String(idGerado)}`);
               const item = result?.items?.[0];
               select?.aoSelecionar?.(item, true);
               if (modal && props.aoSelecionar) props.aoSelecionar(item, true);
            } else if ((form && form.aoSelecionar) || (modal && props.aoSelecionar)) {
               const result = await api.getAll(`${url}?id=${String(idGerado)}`);
               const item = result?.items?.[0];
               form?.aoSelecionar?.(item, true);
               if (modal && props.aoSelecionar) props.aoSelecionar(item, true);
            } else {
               if (!props.id) {
                  await setMany({
                     navegando: true,
                     alterando: false,
                     incluindo: false,
                     itens: [],
                     itemSelecionado: getItemVazio(),
                  });
                  filtrar();
               }
            }
         } else if (formState.alterando) {
            const putFn = isProtected ? api.protectedPut.bind(api) : api.put.bind(api);
            await putFn(url, input);

            if (aposSalvar) await Promise.resolve(aposSalvar(formState, setFormState));

            if ((select && select.aoSelecionar) || (modal && props.aoSelecionar)) {
               const result = await api.getAll(`${url}?id=${String(input.id)}`);
               const item = result?.items?.[0];
               select?.aoSelecionar?.(item, true);
               if (modal && props.aoSelecionar) props.aoSelecionar(item, true);
            } else {
               if (!props.id) {
                  await setMany({
                     navegando: true,
                     alterando: false,
                     incluindo: false,
                     itens: [],
                     itemSelecionado: getItemVazio(),
                     consultou: true,
                  });
               }
               await showInfo('Salvo com sucesso!');
               filtrar();
            }
         }
      } catch (e) {
         await setMany({ botaoSalvarHabilitado: true });
      }
   };

   // ---------- filtros (corrige click e Enter) ----------
   const renderizarFiltros = () => {
      console.log('Form.renderizarFiltros()');
      if (props.id || esconderFiltro) return null;

      return (
         <BootstrapForm.Group style={{ margin: 0, minWidth: 300 }}>
            <InputGroup>
               {formState.filtro?.texto && (
                  <InputGroup.Text
                     as='button'
                     type='button'
                     style={{ padding: '0px 12px 0px 12px', cursor: 'pointer' }}
                     title='Limpar'
                     onClick={limparFiltro}
                  >
                     x
                  </InputGroup.Text>
               )}

               <BootstrapForm.Control
                  ref={textoFiltroRef}
                  type='text'
                  placeholder='Filtrar...'
                  defaultValue={formState.filtro?.texto || ''}
                  onKeyDown={(e) => e.key === 'Enter' && filtrar()}
                  onChange={(e) => {
                     const filtro = { ...(formState.filtro || {}) };
                     filtro.texto = e.target.value;
                     setFormState((prev) => ({ ...prev, filtro }));
                  }}
                  style={{ outline: 'none', boxShadow: 'none', borderColor: '#ced4da', height: 33, fontSize: 13 }}
               />

               <InputGroup.Text
                  as='button'
                  type='button'
                  style={{ padding: '1px 6px 1px 6px', cursor: 'pointer' }}
                  onClick={filtrar}
                  title='Pesquisar'
               >
                  <FontAwesomeIcon className='custom-hover' style={{ fontSize: 22, paddingTop: 2 }} icon={faSearch} />
               </InputGroup.Text>
            </InputGroup>
         </BootstrapForm.Group>
      );
   };

   const getCabecalho = () => {
      return (
         <Navbar
            expand='md'
            style={{
               paddingLeft: 10,
               paddingRight: 10,
               paddingBottom: 2,
               paddingTop: 0,
               marginBottom: 0,
               borderBottom: '1px solid rgb(206, 212, 218)',
               backgroundColor: 'whitesmoke',
            }}
         >
            {!props.esconderTitulo && (
               <span
                  className='navbar-brand'
                  style={{ color: LayoutParams.colors.corSecundaria, fontSize: 26, paddingTop: 6 }}
               >
                  <div style={{ display: 'flex', flexDirection: 'row', flexWrap: 'wrap' }}>
                     <div
                        style={{ cursor: 'pointer', color: LayoutParams.colors.corDoTituloDoFormularioPadrao }}
                        onClick={filtrar}
                     >
                        {titulo}
                     </div>
                     <div style={{ paddingLeft: 8 }}>
                        {formState.navegando && podeIncluir && (
                           <div style={{ display: 'table-cell', marginLeft: '80%', width: '100%' }}>
                              <div title='Novo F1'>
                                 <FontAwesomeIcon
                                    className='custom-hover'
                                    onClick={inserirClick}
                                    icon={faPlusCircle}
                                    style={{
                                       fontSize: 22,
                                       color: LayoutParams.colors.corDosBotoesDoFormularioPadrao,
                                       cursor: 'pointer',
                                    }}
                                 />
                              </div>
                           </div>
                        )}
                     </div>
                  </div>
               </span>
            )}

            {formState.navegando && (
               <>
                  {!props.esconderTitulo && (
                     <>
                        <Navbar.Toggle aria-controls='basic-navbar-nav' />
                        <Navbar.Collapse id='basic-navbar-nav' className='justify-content-end'>
                           <Filler height={10} />
                           {renderizarFiltros()}
                           <Filler height={10} />
                        </Navbar.Collapse>
                     </>
                  )}
                  {props.esconderTitulo && renderizarFiltros()}
               </>
            )}

            {(formState.incluindo || formState.alterando) && (
               <div style={{ display: 'table-cell', width: '100%', textAlign: 'end' }}>
                  <ButtonGroup
                     className='div-acoes-do-formulario'
                     style={{ marginLeft: 'auto', marginRight: 0, marginTop: 5, borderRadius: 4 }}
                  >
                     {!props.id && (
                        <Button
                           id='btnCancelar'
                           icon={<FontAwesomeIcon icon={faTimesCircle} />}
                           onClick={cancelarClick}
                           style={{ width: 130, display: 'flex', padding: '4px 2px 2px 2px', height: 33 }}
                           title={`Cancelar ${formState.incluindo ? 'Inclusão' : 'Alterações'}`}
                           text={podeAlterar ? 'Cancelar' : 'Fechar'}
                        />
                     )}
                     {((formState.incluindo && podeIncluir) || (formState.alterando && podeAlterar)) && (
                        <Button
                           id='btnSalvar'
                           icon={<FontAwesomeIcon icon={faSave} />}
                           disabled={!formState.botaoSalvarHabilitado}
                           onClickAsync={salvar}
                           style={{ width: 130, display: 'flex', padding: '4px 2px 2px 2px', height: 33 }}
                           text='Salvar'
                           inProgressText='Salvando'
                        />
                     )}
                  </ButtonGroup>
               </div>
            )}
         </Navbar>
      );
   };

   const renderizarCabecalhoAcoes = () =>
      select || form ? null : (
         <td className='acoes'>
            <div>Ações</div>
         </td>
      );

   // --- ações em tabela (desktop) ---
   const renderizarAcoesCelula = (item) => {
      if (select || form) return null;
      return (
         <td key='acoes' className='acoes'>
            <div style={{ display: 'flex', justifyContent: 'center', flexWrap: 'wrap' }}>
               <div style={{ display: 'table-cell' }}>
                  {podeAlterar && (
                     <BotaoAlterarItemDeCadastro
                        onClick={(e) => {
                           e.stopPropagation();
                           editarClick(item);
                        }}
                        title='Alterar'
                     />
                  )}
                  {!podeAlterar && (
                     <BotaoVisualizarItemDeCadastro
                        onClick={(e) => {
                           e.stopPropagation();
                           editarClick(item);
                        }}
                        title='Detalhes'
                     />
                  )}
               </div>
               {podeExcluir && (
                  <div style={{ display: 'table-cell' }}>
                     <BotaoExcluirItemDeCadastro
                        onClick={(e) => {
                           e.stopPropagation();
                           excluirClick(item);
                        }}
                        title='Excluir'
                     />
                  </div>
               )}
               {props.getAcoesDaTabela ? props.getAcoesDaTabela(item) : null}
            </div>
         </td>
      );
   };

   // --- ações em cartão (mobile) ---
   const renderizarAcoesInline = (item) => {
      if (select || form) return null;
      return (
         <div style={{ display: 'flex', justifyContent: 'end', gap: 8, marginTop: 6, marginBottom: 6 }}>
            {!podeAlterar && (
               <BotaoVisualizarItemDeCadastro
                  onClick={(e) => {
                     e.stopPropagation();
                     editarClick(item);
                  }}
                  title='Detalhes'
                  size={30}
               />
            )}
            {podeAlterar && (
               <BotaoAlterarItemDeCadastro
                  onClick={(e) => {
                     e.stopPropagation();
                     editarClick(item);
                  }}
                  title='Alterar'
                  size={30}
               />
            )}
            {podeExcluir && (
               <BotaoExcluirItemDeCadastro
                  onClick={(e) => {
                     e.stopPropagation();
                     excluirClick(item);
                  }}
                  title='Excluir'
                  size={30}
               />
            )}
            {props.getAcoesDaTabela ? props.getAcoesDaTabela(item) : null}
         </div>
      );
   };

   const onRowClick = (item) => {
      if (select?.aoSelecionar) select.aoSelecionar(item);
      if (form?.aoSelecionar) form.aoSelecionar(item);
   };

   const doDoubleClickAction = (item) => {
      if (select || form || apenasInserir) return;
      editarClick(item);
   };

   const handleClick = (item) => {
      timer = setTimeout(function () {
         if (!prevent) onRowClick(item);
         prevent = false;
      }, delay);
   };

   const handleDoubleClick = (item) => {
      clearTimeout(timer);
      prevent = true;
      doDoubleClickAction(item);
   };

   const getCabecalhosDaTabela = () => {
      const mobile = isMobile();
      const titulos = getTitulosDaTabela?.();
      if (!titulos || mobile) return null;

      return (
         <div className='div-cabecalho-tabela-formulario-padrao'>
            <table className='cabecalho-tabela-formulario-padrao'>
               <thead>
                  <tr>
                     {titulos.map((item, index) => {
                        if (!mostrarCodigo && index === 0) return null;
                        return (
                           <td
                              key={index}
                              className={item.className}
                              onClick={() => {
                                 if (item.orderby) {
                                    let ordenacao = item.orderby;
                                    if (ordenacao === formState.ordenacao) ordenacao += ' desc';
                                    setMany({ ordenacao }).then(filtrar);
                                 }
                              }}
                              style={{ cursor: item.orderby ? 'pointer' : 'default', width: item.width }}
                              title={item.titulo}
                           >
                              {item.titulo}
                           </td>
                        );
                     })}
                     {renderizarCabecalhoAcoes()}
                  </tr>
               </thead>
            </table>
         </div>
      );
   };

   const getLista = () => {
      const mobile = isMobile();
      const titulos = getTitulosDaTabela?.();
      if (!titulos) return null;

      const tamanhos = titulos.map((i) => i.width);
      const classes = titulos.map((i) => i.className);
      const textoDosTitulos = titulos.map((i) => i.titulo);

      if (mobile) {
         if (formState.vazio) {
            return (
               <table className='tabela-formulario-padrao'>
                  <tbody>
                     <tr>
                        <td style={{ width: '100%', textAlign: 'center' }}>
                           <span>{formState.carregando ? 'Carregando...' : 'Nenhum registro encontrado'}</span>
                        </td>
                     </tr>
                  </tbody>
               </table>
            );
         }
         return formState.itens.map((item, rowIndex) => {
            const dados = getDadosDaTabela(item);
            if (!item || !dados) return null;
            return (
               <div
                  key={rowIndex}
                  className={select || form ? 'noselect' : undefined}
                  style={{
                     cursor: select || form ? 'pointer' : 'default',
                     border: '1px solid rgb(206, 212, 218)',
                     borderRadius: 5,
                     margin: 5,
                     padding: 3,
                  }}
                  onDoubleClick={() => handleDoubleClick(item)}
                  onClick={() => handleClick(item)}
               >
                  {dados.map(
                     (dado, campoIndex) =>
                        dado && (
                           <div key={campoIndex} style={{ display: 'flex', flexDirection: 'row' }}>
                              <div style={{ display: 'table-cell', fontWeight: 600 }}>
                                 {textoDosTitulos[campoIndex]}:&nbsp;
                              </div>
                              <div style={{ display: 'table-cell' }}>
                                 <span style={{ wordWrap: 'anywhere' }}>{dado}</span>
                              </div>
                           </div>
                        )
                  )}
                  {renderizarAcoesInline(item)} {/* <<<<< mobile: div, não <td> */}
               </div>
            );
         });
      }

      if (formState.vazio) {
         return (
            <table className='tabela-formulario-padrao'>
               <tbody>
                  <tr>
                     <td style={{ width: '100%', textAlign: 'center', color: '#999' }}>
                        <span>{formState.carregando ? 'Carregando...' : 'Nenhum registro encontrado'}</span>
                     </td>
                  </tr>
               </tbody>
            </table>
         );
      }

      return (
         <table className='tabela-formulario-padrao table-hover'>
            <tbody>
               {formState.itens.map((item, rowIndex) => {
                  const dados = getDadosDaTabela(item);
                  if (!item || !dados) return <tr key={rowIndex}>{[renderizarAcoesCelula(item)]}</tr>;
                  return (
                     <tr
                        key={rowIndex}
                        className={select || form ? 'noselect' : undefined}
                        style={{ cursor: select || form ? 'pointer' : 'default' }}
                        onDoubleClick={() => handleDoubleClick(item)}
                        onClick={() => handleClick(item)}
                     >
                        {dados.map((dado, campoIndex) => {
                           if (!mostrarCodigo && campoIndex === 0) return null;
                           return (
                              <td
                                 key={campoIndex}
                                 className={classes[campoIndex]}
                                 style={{ width: tamanhos[campoIndex] }}
                              >
                                 {dado}
                              </td>
                           );
                        })}
                        {renderizarAcoesCelula(item)}
                     </tr>
                  );
               })}
            </tbody>
         </table>
      );
   };

   const getFormulario = () => {
      return (
         <Row
            className={props.formularioClassName || 'justify-content-md-center mx-0'}
            style={{ overflowY: props.overflowY || 'auto', overflowX: props.overflowX || 'hidden', paddingTop: 5 }}
         >
            <Col style={{ maxWidth: props.maxWidth || 800, minHeight: props.minHeight || 400 }}>
               <fieldset disabled={formState.incluindo || (formState.alterando && podeAlterar) ? false : true}>
                  {renderizarFormularioProp?.({
                     formState,
                     setFormState,
                     salvar,
                     cancelar: cancelarClick,
                     editar: editarClick,
                     setMany,
                  })}
               </fieldset>
            </Col>
         </Row>
      );
   };

   const getNavegador = () => {
      return (
         <div
            style={{
               width: '100%',
               display: 'flex',
               justifyContent: 'flex-end',
               padding: '10px 2px 10px 10px',
               position: 'relative',
            }}
         >
            {(select || form || modal) && !formState.incluindo && (
               <div style={{ width: 150, textAlign: 'left' }}>
                  <Button variant='secondary' onClick={cancelarClick} style={{ width: 150 }} text='Fechar' />
               </div>
            )}
            {formState.itens && (
               <>
                  <div style={{ width: '100%', textAlign: 'left', paddingTop: 10, paddingLeft: 5 }}>
                     {formState.quantidadeTotalDeDados ? (
                        <div>
                           <span>Mostrando&nbsp;</span>
                           <span>{formState.quantidadeDeDados}</span>
                           <span>&nbsp;de&nbsp;</span>
                           <span>{formState.quantidadeTotalDeDados}</span>
                        </div>
                     ) : null}
                  </div>
                  <ButtonGroup className='mr-2' style={{ minWidth: 120 }}>
                     <Button
                        title='Carregar mais'
                        text=''
                        icon={<FontAwesomeIcon icon={faAngleDown} />}
                        variant='secondary'
                        onClick={() => navegar(1)}
                        disabled={!formState.podeAvancar}
                        style={{ cursor: formState.podeAvancar ? 'pointer' : 'not-allowed', padding: 0 }}
                     />
                     <Button
                        title='Carregar todos'
                        text=''
                        icon={<FontAwesomeIcon icon={faAngleDoubleDown} />}
                        variant='secondary'
                        onClick={() => navegar(2)}
                        disabled={!formState.podeCarregarTodos}
                        style={{ cursor: formState.podeCarregarTodos ? 'pointer' : 'not-allowed', padding: 0 }}
                     />
                  </ButtonGroup>
               </>
            )}
         </div>
      );
   };

   useImperativeHandle(ref, () => ({
      filtrar,
      navegar,
      inserirClick,
      editarClick,
      cancelarClick,
      salvar,
      getFormState: () => formState,
      setFormState: (patch) => setMany(patch),
   }));

   const content = (
      <div
         id='formularioPadrao'
         ref={rootRef}
         style={{
            display: 'flex',
            flexDirection: 'column',
            maxHeight: '100%',
            overflowX: 'hidden',
            width: '100%',
            maxWidth: '100%',
            height: '100%',
         }}
         tabIndex={0}
         onKeyDown={(e) => {
            if (e.keyCode === 112) inserirClick(); // F1
            if (e.keyCode === 115 && (formState.incluindo || formState.alterando)) salvar(); // F4
            if (e.keyCode === 27) {
               if (formState.navegando) limparFiltro();
               if (formState.incluindo || formState.alterando) cancelarClick();
            }
         }}
      >
         {getCabecalho()}
         {formState.navegando && getCabecalhosDaTabela()}
         {formState.navegando && <div className='div-tabela-formulario-padrao'>{getLista()}</div>}
         {(formState.incluindo || formState.alterando) &&
            formState.mostrarFormulario &&
            renderizarFormularioProp &&
            getFormulario()}
         {formState.navegando && getNavegador()}
      </div>
   );

   if (modal) {
      return (
         <Modal
            show
            scrollable
            size='lg'
            onHide={() => {}}
            onKeyDown={(e) => {
               if (e.keyCode === 27) setFormState({ incluindo: false });
            }}
            dialogClassName='h-100'
         >
            <Modal.Body
               style={{
                  overflow: 'hidden',
                  display: 'flex',
                  position: 'relative',
                  fontSize: 13,
                  padding: 0,
                  maxHeight: '100%',
               }}
            >
               {content}
            </Modal.Body>
         </Modal>
      );
   }

   return content;
});

export default Form;

export const makeFormHelpers = (setFormState) => ({
   setItemSelecionado: (updatesOrFn) => {
      if (typeof updatesOrFn === 'function') {
         setFormState((prev) => ({
            ...prev,
            itemSelecionado: { ...prev.itemSelecionado, ...updatesOrFn(prev.itemSelecionado || {}) },
         }));
      } else {
         setFormState((prev) => ({
            ...prev,
            itemSelecionado: { ...prev.itemSelecionado, ...updatesOrFn },
         }));
      }
   },
});
