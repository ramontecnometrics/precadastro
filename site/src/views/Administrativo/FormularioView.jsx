import React, { useMemo, useRef } from 'react';
import { Row, Col } from '../../components/Grid';
import FormGroup from '../../components/FormGroup';
import Form, { makeFormHelpers } from '../../components/forms/Form';
import TextInput from '../../components/TextInput';
import FormularioController from './FormularioController';
import Filler from '../../components/Filler';

import BoldLabel from '../../components/BoldLabel';
import FormDetails from '../../components/forms/FormDetails';
import TextArea from '../../components/TextArea';
import CheckBox from '../../components/CheckBox';
import Select from '../../components/Select';
import IntegerInput from '../../components/IntegerInput';
import Panel from '../../components/Panel';
import IconButton from '../../components/IconButton';
import { faPlusCircle, faTimes } from '@fortawesome/free-solid-svg-icons';
import { FlexRow } from '../../components/FlexItems';
import { FlexCol } from '../../components/FlexItems';
import Text from '../../components/Text';

const url = '/formulario';

export default function FormularioView(props) {
   const controller = useMemo(() => new FormularioController(), []);
   const autoFocusRef = useRef(null);
   const autoFocusGrupoRef = useRef(null);
   const autoFocusCampoRef = useRef(null);

   const getOpcoes = (opcoes) => {
      let result = [];

      if (opcoes) {
         let count = opcoes.split('|').length - 1;
         result = opcoes.split('|', count);
      }
      return result;
   };

   const adicionarOpcao = (opcoes) => {
      if (opcoes == null || opcoes == undefined) {
         return 'Opção 1| ';
      }

      return opcoes + '' + opcoes.split('|').length + '|';
   };

   const removerOpcao = (opcoes, index) => {
      var opcoesArray = opcoes.split('|');
      opcoesArray.splice(index, 1);
      return opcoesArray.join('|');
   };

   const atualizarOpcao = (opcoes, index, valor) => {
      var opcoesArray = opcoes.split('|');
      opcoesArray[index] = valor;
      return opcoesArray.join('|');
   };

   const renderizarFormulario = ({ formState, setFormState }) => {
      const { setItemSelecionado } = makeFormHelpers(setFormState);

      const itemSelecionado = formState.itemSelecionado;
      const grupoSelecionado = formState.grupoSelecionado;
      const campoSelecionado = formState.campoSelecionado;

      return (
         <>
            <Row>
               <Col sm={2}>
                  <FormGroup>
                     <BoldLabel>Código</BoldLabel>
                     <TextInput readOnly defaultValue={itemSelecionado.id || ''} />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <BoldLabel>Nome</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.nome}
                        onChange={(value) => setItemSelecionado({ nome: value })}
                        ref={autoFocusRef}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col>
                  <FormGroup>
                     <BoldLabel>Descrição</BoldLabel>
                     <TextArea
                        defaultValue={itemSelecionado.descricao}
                        onChange={(value) => setItemSelecionado({ descricao: value })}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Filler height={10} />

            <FormDetails
               titulo='Sessões'
               itens={itemSelecionado.grupos || []}
               autoFocus={autoFocusGrupoRef}
               novo={() =>
                  setFormState((prev) => ({
                     ...prev,
                     grupoSelecionado: {
                        ordem:
                           (itemSelecionado.grupos || []).length === 0 ? 1 : itemSelecionado.grupos.at(-1).ordem + 1,
                     },
                  }))
               }
               cancelar={() => setFormState((prev) => ({ ...prev, grupoSelecionado: null }))}
               inserir={() => controller.inserirGrupo(formState, setFormState, itemSelecionado, setItemSelecionado)}
               alterar={(indiceEmEdicao) =>
                  controller.alterarGrupo(formState, setFormState, itemSelecionado, setItemSelecionado, indiceEmEdicao)
               }
               selecionarParaAlteracao={(index) =>
                  setFormState((prev) => ({
                     ...prev,
                     grupoSelecionado: prev.itemSelecionado.grupos[index],
                  }))
               }
               excluir={(index) =>
                  setItemSelecionado({
                     grupos: (itemSelecionado.grupos || []).filter((_, i) => i !== index),
                  })
               }
               aposSalvar={() => {
                  const gruposOrdenados = [...(itemSelecionado.grupos || [])].sort(
                     (a, b) => Number(a.ordem) - Number(b.ordem)
                  );

                  setItemSelecionado({
                     grupos: gruposOrdenados,
                  });
               }}
               colunas={() => [
                  { titulo: 'Descrição', width: '80%' },
                  { titulo: 'Ordem', width: '20%', className: 'center' },
               ]}
               linha={(item) => [item.titulo || '', item.ordem]}
               formulario={() =>
                  grupoSelecionado && (
                     <>
                        <Row>
                           <Col>
                              <FormGroup>
                                 <BoldLabel>Título da sessão</BoldLabel>
                                 <TextInput
                                    defaultValue={grupoSelecionado.titulo}
                                    onChange={(value) => {
                                       setFormState((prev) => ({
                                          ...prev,
                                          grupoSelecionado: { ...prev.grupoSelecionado, titulo: value },
                                       }));
                                    }}
                                    ref={autoFocusGrupoRef}
                                 />
                              </FormGroup>
                           </Col>
                           <Col sm={2} md={2} lg={2} xl={2}>
                              <FormGroup>
                                 <BoldLabel>Ordem</BoldLabel>
                                 <IntegerInput
                                    defaultValue={grupoSelecionado.ordem}
                                    onChange={(value) => {
                                       setFormState((prev) => ({
                                          ...prev,
                                          grupoSelecionado: { ...prev.grupoSelecionado, ordem: value },
                                       }));
                                    }}
                                 />
                              </FormGroup>
                           </Col>
                        </Row>
                        <Filler height={10} />

                        <FormDetails
                           titulo='Campos'
                           modal={true}
                           itens={grupoSelecionado.campos || []}
                           novo={() => {
                              setFormState((prev) => ({
                                 ...prev,
                                 campoSelecionado: {
                                    obrigatorio: true,
                                    ordem:
                                       (grupoSelecionado.campos || []).length === 0
                                          ? 1
                                          : grupoSelecionado.campos.at(-1).ordem + 1,
                                 },
                              }));
                           }}
                           autoFocus={autoFocusCampoRef}
                           cancelar={() => setFormState((prev) => ({ ...prev, campoSelecionado: null }))}
                           inserir={() =>
                              controller.inserirCampo(formState, setFormState, itemSelecionado, setItemSelecionado)
                           }
                           alterar={(indiceEmEdicao) =>
                              controller.alterarCampo(
                                 formState,
                                 setFormState,
                                 campoSelecionado,
                                 setItemSelecionado,
                                 indiceEmEdicao
                              )
                           }
                           selecionarParaAlteracao={(index) =>
                              setFormState((prev) => ({
                                 ...prev,
                                 campoSelecionado: prev.grupoSelecionado.campos[index],
                              }))
                           }
                           excluir={(index) =>
                              setFormState((prev) => ({
                                 ...prev,
                                 grupoSelecionado: {
                                    ...prev.grupoSelecionado,
                                    campos: (prev.grupoSelecionado.campos || []).filter((_, i) => i !== index),
                                 },
                              }))
                           }
                           aposSalvar={() => {}}
                           colunas={() => [
                              { titulo: 'Descrição', width: '80%' },
                              { titulo: 'Ordem', width: '20%', className: 'center' },
                           ]}
                           linha={(item) => [item.titulo || '', item.ordem]}
                           formulario={() =>
                              campoSelecionado && (
                                 <>
                                    <Row>
                                       <Col>
                                          <FormGroup>
                                             <BoldLabel>Campo</BoldLabel>
                                             <TextInput
                                                ref={autoFocusCampoRef}
                                                defaultValue={campoSelecionado.titulo}
                                                onChange={(value) => {
                                                   setFormState((prev) => ({
                                                      ...prev,
                                                      campoSelecionado: { ...prev.campoSelecionado, titulo: value },
                                                   }));
                                                }}
                                             />
                                          </FormGroup>
                                       </Col>
                                       <Col sm={2} md={2} lg={2} xl={2}>
                                          <FormGroup>
                                             <BoldLabel>Ordem</BoldLabel>
                                             <IntegerInput
                                                defaultValue={campoSelecionado.ordem}
                                                onChange={(value) => {
                                                   setFormState((prev) => ({
                                                      ...prev,
                                                      campoSelecionado: { ...prev.campoSelecionado, ordem: value },
                                                   }));
                                                }}
                                             />
                                          </FormGroup>
                                       </Col>
                                    </Row>
                                    <Row>
                                       <Col>
                                          <FormGroup>
                                             <BoldLabel>Tipo</BoldLabel>
                                             <Select
                                                name='tipo'
                                                defaultValue={campoSelecionado.tipo}
                                                options={[
                                                   { id: 'texto', descricao: 'Texto livre' },
                                                   { id: 'simnao', descricao: 'Sim/Não' },
                                                   { id: 'opcoes', descricao: 'Opções' },
                                                   { id: 'opcoesmultiplas', descricao: 'Opções múltiplas' },
                                                   { id: 'selecao', descricao: 'Lista' },
                                                ]}
                                                getKeyValue={(i) => i.id}
                                                getDescription={(i) => i.descricao}
                                                onSelect={(i) =>
                                                   setFormState((prev) => ({
                                                      ...prev,
                                                      campoSelecionado: {
                                                         ...prev.campoSelecionado,
                                                         tipo: i ? i.id : null,
                                                      },
                                                   }))
                                                }
                                             />
                                          </FormGroup>
                                       </Col>
                                       <Col>
                                          <FormGroup>
                                             <Filler height={38} />
                                             <CheckBox
                                                defaultChecked={campoSelecionado.obrigatorio}
                                                label={'Obrigatório'}
                                                onChange={(value) => {
                                                   setFormState((prev) => ({
                                                      ...prev,
                                                      campoSelecionado: {
                                                         ...prev.campoSelecionado,
                                                         obrigatorio: value,
                                                      },
                                                   }));
                                                }}
                                             />
                                          </FormGroup>
                                       </Col>
                                    </Row>

                                    {(campoSelecionado.tipo === 'opcoes' ||
                                       campoSelecionado.tipo === 'opcoesmultiplas' ||
                                       campoSelecionado.tipo === 'selecao') && (
                                       <Row>
                                          <Col>
                                             <Panel style={{ padding: 10 }}>
                                                <BoldLabel>Opções:</BoldLabel>
                                                <div
                                                   style={{
                                                      display: 'flex',
                                                      flexDirection: 'row',
                                                      gap: 5,
                                                      flexWrap: 'wrap',
                                                   }}
                                                >
                                                   {getOpcoes(campoSelecionado.opcoes).map((opcao, index) => {
                                                      return (
                                                         <>
                                                            <FlexRow
                                                               style={{
                                                                  width: 120,
                                                                  border: '1px solid gray',
                                                                  padding: 3,
                                                                  borderRadius: 6,
                                                               }}
                                                               gap={5}
                                                            >
                                                               <FlexCol style={{ width: '80%' }}>
                                                                  <input
                                                                     type='text'
                                                                     style={{
                                                                        border: 0,
                                                                        maxWidth: '100%',
                                                                        outline: 'none',
                                                                     }}
                                                                     value={opcao}
                                                                     onChange={(e) => {
                                                                        setFormState((prev) => ({
                                                                           ...prev,
                                                                           campoSelecionado: {
                                                                              ...prev.campoSelecionado,
                                                                              opcoes: atualizarOpcao(
                                                                                 prev.campoSelecionado.opcoes,
                                                                                 index,
                                                                                 e.target.value
                                                                              ),
                                                                           },
                                                                        }));
                                                                     }}
                                                                  />
                                                               </FlexCol>
                                                               <FlexCol style={{ width: '20%' }}>
                                                                  <IconButton
                                                                     icon={faTimes}
                                                                     onClick={() => {
                                                                        setFormState((prev) => ({
                                                                           ...prev,
                                                                           campoSelecionado: {
                                                                              ...prev.campoSelecionado,
                                                                              opcoes: removerOpcao(
                                                                                 prev.campoSelecionado.opcoes,
                                                                                 index
                                                                              ),
                                                                           },
                                                                        }));
                                                                     }}
                                                                  />
                                                               </FlexCol>
                                                            </FlexRow>
                                                         </>
                                                      );
                                                   })}
                                                   <IconButton
                                                      style={{ fontSize: 22, marginTop: 4 }}
                                                      icon={faPlusCircle}
                                                      title={'Adicionar opção'}
                                                      onClick={() => {
                                                         setFormState((prev) => ({
                                                            ...prev,
                                                            campoSelecionado: {
                                                               ...prev.campoSelecionado,
                                                               opcoes: adicionarOpcao(prev.campoSelecionado.opcoes),
                                                            },
                                                         }));
                                                      }}
                                                   />
                                                </div>
                                             </Panel>
                                          </Col>
                                       </Row>
                                    )}
                                 </>
                              )
                           }
                        />
                        <Filler height={10} />
                     </>
                  )
               }
            />
         </>
      );
   };

   return (
      <Form
         titulo='Formulários'
         url={url}
         fastUrl={`${url}/fast`}
         ordenacaoPadrao='nome'
         permissoes={[1061, 1062, 1063, 1064]}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         antesDeInserir={controller.antesDeInserir}
         antesDeEditar={controller.antesDeEditar}
         select={props.select}
         itemVazio={controller.itemVazio}
         autoFocus={autoFocusRef}
      />
   );
}
