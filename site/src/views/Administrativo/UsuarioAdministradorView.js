import React, { useMemo } from 'react';
import { Row, Col } from '../../components/Grid';
import FormGroup from '../../components/FormGroup';
import { showConfirm } from '../../components/Messages';
import Form, { makeFormHelpers } from '../../components/forms/Form';
import Avatar from '../../components/Avatar';
import TextInput from '../../components/TextInput';
import UsuarioAdministradorController from './UsuarioAdministradorController';
import { buildQueryString, generatePassword } from '../../utils/Functions';
import Select from '../../components/Select';
import CheckBox from '../../components/CheckBox';
import Filler from '../../components/Filler';
import PasswordInput from '../../components/PasswordInput';
import { faKey, faTrash } from '@fortawesome/free-solid-svg-icons';
import EmailInput from '../../components/EmailInput';
import BoldLabel from '../../components/BoldLabel';
import FormDetails from '../../components/forms/FormDetails';
import PerfilDeUsuarioView from './PerfilDeUsuarioView';
import TextArea from '../../components/TextArea';

const url = '/usuarioadministrador';

export default function UsuarioAdministradorView(props) {
   const controller = useMemo(() => new UsuarioAdministradorController(), []);

   const renderizarFormulario = ({ formState, setFormState }) => {
      const { setItemSelecionado } = makeFormHelpers(setFormState);

      const itemSelecionado = formState.itemSelecionado;
      const alterando = !!formState.alterando;

      return (
         <>
            <fieldset disabled={itemSelecionado.nome?.toUpperCase() === 'MASTER'}>
               <Row>
                  <Col
                     name={'foto-small'}
                     className='show-on-small-screen'
                     sm={12}
                     md={12}
                     style={{ textAlign: 'center' }}
                  >
                     <div style={{ margin: 'auto', width: 'fit-content' }}>
                        <Avatar
                           image={itemSelecionado.foto}
                           onChange={(image) => setItemSelecionado({ foto: image })}
                        />
                     </div>
                  </Col>
                  <Col>
                     <Row>
                        <Col sm={2}>
                           <FormGroup>
                              <BoldLabel>Código</BoldLabel>
                              <TextInput readOnly defaultValue={itemSelecionado.id || ''} />
                           </FormGroup>
                        </Col>
                        <Col>
                           <FormGroup>
                              <BoldLabel>Situação</BoldLabel>
                              <Select
                                 as='select'
                                 name='situacao'
                                 defaultValue={itemSelecionado.situacao}
                                 options={[
                                    { id: 1, descricao: 'Ativo' },
                                    { id: 2, descricao: 'Inativo' },
                                 ]}
                                 getKeyValue={(i) => i.id}
                                 getDescription={(i) => i.descricao}
                                 onSelect={(i) => setItemSelecionado({ situacao: i })}
                                 readOnly={itemSelecionado.nome?.toUpperCase() === 'MASTER'}
                              />
                           </FormGroup>
                        </Col>
                     </Row>

                     <Row>
                        <Col>
                           <FormGroup>
                              <BoldLabel>Nome</BoldLabel>
                              <TextInput
                                 defaultValue={itemSelecionado.nome}
                                 onChange={(value) => setItemSelecionado({ nome: value })}
                                 upperCase
                              />
                           </FormGroup>
                        </Col>
                        {/* <Col sm={4}>
                        <FormGroup>
                           <Label>Apelido</Label>
                           <TextInput
                              defaultValue={itemSelecionado.apelido || ''}
                              onChange={(value) => setItemSelecionado({ apelido: value })}
                              upperCase
                           />
                        </FormGroup>
                     </Col> */}
                     </Row>
                  </Col>

                  <Col
                     name={'foto-on-large-screen'}
                     className='hide-on-small-screen'
                     style={{ textAlign: 'right', maxWidth: 110 }}
                  >
                     <Avatar image={itemSelecionado.foto} onChange={(image) => setItemSelecionado({ foto: image })} />
                  </Col>
               </Row>
               {itemSelecionado.nome?.toUpperCase() !== 'MASTER' && (
                  <>
                     <Row>
                        <Col>
                           <FormGroup>
                              <BoldLabel>Email</BoldLabel>
                              <EmailInput
                                 defaultValue={itemSelecionado.email || ''}
                                 onChange={(value) => setItemSelecionado({ email: value })}
                              />
                           </FormGroup>
                        </Col>
                     </Row>

                     <Row>
                        <Col sm={6}>
                           <FormGroup>
                              <BoldLabel>Login</BoldLabel>
                              <TextInput
                                 readOnly={alterando}
                                 defaultValue={itemSelecionado.nomeDeUsuario || ''}
                                 onChange={(value) => setItemSelecionado({ nomeDeUsuario: value })}
                              />
                           </FormGroup>
                        </Col>

                        <Col sm={6}>
                           <FormGroup>
                              <BoldLabel>Senha</BoldLabel>

                              <>
                                 <PasswordInput
                                    defaultValue={itemSelecionado.senha}
                                    readOnly={!!(itemSelecionado.senha && !itemSelecionado.senhaAlterada)}
                                    onChange={(value) =>
                                       setItemSelecionado({
                                          senha: value,
                                          senhaAlterada: true,
                                          enviarNovaSenhaPorEmail: false,
                                       })
                                    }
                                    appendIcon={itemSelecionado.senha ? faTrash : faKey}
                                    appendTitle={itemSelecionado.senha ? 'Limpar senha' : 'Gerar nova senha'}
                                    onAppendClick={() => {
                                       if (itemSelecionado.senha) {
                                          showConfirm('Deseja limpar a senha?', () => {
                                             setItemSelecionado({ senha: null, senhaAlterada: true });
                                          });
                                       } else {
                                          showConfirm('Deseja realmente gerar uma nova senha?', () => {
                                             setItemSelecionado({
                                                senha: generatePassword(true, 8),
                                                senhaAlterada: true,
                                                enviarNovaSenhaPorEmail: true,
                                             });
                                          });
                                       }
                                    }}
                                 />

                                 {itemSelecionado.senhaAlterada && itemSelecionado.senha && (
                                    <>
                                       <Filler height={2} />
                                       <CheckBox
                                          checked={!!itemSelecionado.enviarNovaSenhaPorEmail}
                                          name='enviarNovaSenhaPorEmail'
                                          label='Enviar nova senha por email'
                                          onChange={(checked) =>
                                             setItemSelecionado({ enviarNovaSenhaPorEmail: checked })
                                          }
                                       />
                                    </>
                                 )}
                              </>
                           </FormGroup>
                        </Col>
                     </Row>

                     <Filler height={10} />

                     <FormDetails
                        titulo='Perfis de usuário'
                        exibirTitulos={false}
                        itens={itemSelecionado.perfis || []}
                        novo={() => setFormState((prev) => ({ ...prev, perfilSelecionado: {} }))}
                        cancelar={() => setFormState((prev) => ({ ...prev, perfilSelecionado: null }))}
                        inserir={() => {
                           return new Promise((resolve, reject) => {
                              const perfil = formState.perfilSelecionado;
                              if (!perfil) return;
                              const atuais = itemSelecionado.perfis || [];
                              const semDuplicados = atuais.filter((p) => p?.perfil?.id !== perfil.id);
                              setItemSelecionado({ perfis: [...semDuplicados, { perfil }] });
                              setFormState((prev) => ({ ...prev, perfilSelecionado: null }));
                              resolve();
                           });
                        }}
                        selecionarParaAlteracao={(index) =>
                           setFormState((prev) => ({
                              ...prev,
                              perfilSelecionado: index !== 0 ? (prev.itemSelecionado.perfis || [])[index] : null,
                           }))
                        }
                        excluir={(index) =>
                           setItemSelecionado({
                              perfis: (itemSelecionado.perfis || []).filter((_, i) => i !== index),
                           })
                        }
                        colunas={() => [{ titulo: 'Descrição', width: '100%' }]}
                        linha={(it) => [it?.perfil?.nome || '']}
                        formulario={() => (
                           <Row>
                              <Col>
                                 <FormGroup>
                                    <BoldLabel>Perfil de usuário</BoldLabel>
                                    <Select
                                       name='perfil'
                                       defaultValue={formState.perfilSelecionado}
                                       getKeyValue={(i) => i.id}
                                       getDescription={(i) => i.nome}
                                       onSelect={(i) => {
                                          setFormState((prev) => ({ ...prev, perfilSelecionado: i }));
                                          console.log(i);
                                       }}
                                       formularioPadrao={(select) => (
                                          <PerfilDeUsuarioView
                                             select={select}
                                             filtroExtra={() => ({
                                                tipoDePerfil: 1, // Néos
                                             })}
                                          />
                                       )}
                                       noDropDown
                                       readOnlyColor='#ffff'
                                       getFilterUrl={(text) =>
                                          '/perfildeusuario/fast' +
                                          buildQueryString(2, null, 'id', { Searchable: text })
                                       }
                                    />
                                 </FormGroup>
                              </Col>
                           </Row>
                        )}
                     />
                     <Filler height={10} />
                     <Row>
                        <Col>
                           <FormGroup>
                              <BoldLabel>Certificado para integração via Api</BoldLabel>
                              <TextArea
                                 defaultValue={itemSelecionado.certificado}
                                 rows={5}
                                 onChange={(value) =>
                                    setFormState((prev) => ({
                                       ...prev,
                                       itemSelecionado: {
                                          ...prev.itemSelecionado,
                                          certificado: value,
                                       },
                                    }))
                                 }
                              />
                           </FormGroup>
                        </Col>
                     </Row>
                  </>
               )}
            </fieldset>
         </>
      );
   };

   return (
      <Form
         titulo='Usuários'
         url={url}
         fastUrl={`${url}/fast`}
         ordenacaoPadrao='nomeDeUsuario'
         permissoes={[1021, 1022, 1023, 1024]}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         antesDeInserir={controller.antesDeInserir}
         antesDeEditar={controller.antesDeEditar}
         select={props.select}
         itemVazio={controller.itemVazio}
      />
   );
}
