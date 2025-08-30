import React, { useMemo } from 'react';
import { Row, Col } from '../../components/Grid';
import FormGroup from '../../components/FormGroup';
import Form, { makeFormHelpers } from '../../components/forms/Form';
import Avatar from '../../components/Avatar';
import TextInput from '../../components/TextInput';
import LeadController from './LeadController';
import Select from '../../components/Select';
import Filler from '../../components/Filler';
import EmailInput from '../../components/EmailInput';
import BoldLabel from '../../components/BoldLabel';
import TextArea from '../../components/TextArea';
import CpfInput from '../../components/CpfInput';
import CepInput from '../../components/CepInput';
import CidadeView from '../CidadeView';
import ProfissaoView from '../ProfissaoView';
import { FlexRow, FlexCol } from '../../components/FlexItems';
import DateInput from '../../components/DateInput';

const url = '/lead';

export default function LeadView(props) {
   const controller = useMemo(() => new LeadController(), []);

   const renderizarFormulario = ({ formState, setFormState }) => {
      const { setItemSelecionado } = makeFormHelpers(setFormState);

      const itemSelecionado = formState.itemSelecionado;      

      return (
         <>
            <Row>
               <Col
                  name={'foto-small'}
                  className='show-on-small-screen'
                  sm={12}
                  md={12}
                  style={{ textAlign: 'center' }}
               >
                  <div style={{ margin: 'auto', width: 'fit-content' }}>
                     <Avatar image={itemSelecionado.foto} onChange={(image) => setItemSelecionado({ foto: image })} />
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
                              allowEmpty={false}
                           />
                        </FormGroup>
                     </Col>
                     <Col>
                        <FormGroup>
                           <BoldLabel>Data de cadastro</BoldLabel>
                           <DateInput
                              defaultValue={itemSelecionado.dataDeCadastro}
                              onChange={(value) => setItemSelecionado({ dataDeCadastro: value })}
                           />
                        </FormGroup>
                     </Col>
                  </Row>

                  <Row>
                     <Col>
                        <FormGroup>
                           <BoldLabel>Nome</BoldLabel>
                           <TextInput
                              defaultValue={itemSelecionado.nomeCompleto}
                              onChange={(value) => setItemSelecionado({ nomeCompleto: value })}
                           />
                        </FormGroup>
                     </Col>
                     <Col md={4} lg={3} xl={4}>
                        <FormGroup>
                           <BoldLabel>Data de nascimento</BoldLabel>
                           <DateInput
                              defaultValue={itemSelecionado.dataDeNascimento}
                              onChange={(value) => setItemSelecionado({ dataDeNascimento: value })}
                           />
                        </FormGroup>
                     </Col>
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

            <Row>
               <Col>
                  <FormGroup>
                     <BoldLabel>CPF</BoldLabel>
                     <CpfInput
                        defaultValue={itemSelecionado.cpf}
                        onChange={(value) => setItemSelecionado({ cpf: value })}
                     />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <BoldLabel>RG</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.documentoDeIdentidade}
                        onChange={(value) => setItemSelecionado({ documentoDeIdentidade: value })}
                        upperCase
                     />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <BoldLabel>CNH</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.cnh}
                        onChange={(value) => setItemSelecionado({ cnh: value })}
                        upperCase
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col>
                  <FormGroup>
                     <BoldLabel>Estado Civil</BoldLabel>
                     <Select
                        as='select'
                        name='estadoCivil'
                        defaultValue={itemSelecionado.estadoCivil}
                        options={[
                           { id: 0, descricao: 'Não informado' },
                           { id: 1, descricao: 'Solteiro(a)' },
                           { id: 2, descricao: 'Casado(a)' },
                           { id: 3, descricao: 'Viúvo(a)' },
                           { id: 4, descricao: 'Divorciado(a)' },
                           { id: 5, descricao: 'Separado(a)' },
                           { id: 6, descricao: 'União Civil' },
                        ]}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.descricao}
                        onSelect={(i) => setItemSelecionado({ estadoCivil: i })}
                     />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <BoldLabel>Profissão</BoldLabel>
                     <Select
                        name='profissao'
                        defaultValue={itemSelecionado.profissao}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setItemSelecionado({ profissao: i })}
                        formularioPadrao={(select) => (
                           <ProfissaoView select={select} filtroExtra={() => ({ situacao: 1 })} />
                        )}
                        noDropDown
                        readOnlyColor='#ffff'
                     />
                  </FormGroup>
               </Col>
               <Col sm={2} md={2} lg={2} xl={2}>
                  <FormGroup>
                     <BoldLabel>Sexo</BoldLabel>
                     <Select                        
                        name='sexo'
                        defaultValue={itemSelecionado.sexo}
                        options={[
                           { id: 1, descricao: 'Masculino' },
                           { id: 2, descricao: 'Feminino' },
                        ]}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.descricao}
                        onSelect={(i) => setItemSelecionado({ sexo: i })}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Filler height={10} />

            <Row>
               <Col>
                  <FormGroup>
                     <BoldLabel>Telefone</BoldLabel>
                     <FlexRow gap={4}>
                        <FlexCol width={70}>
                           <TextInput
                              placeholder='DDD'
                              defaultValue={itemSelecionado.telefone?.ddd}
                              onChange={(value) =>
                                 setItemSelecionado({
                                    telefone: { ...itemSelecionado.telefone, ddd: value },
                                 })
                              }
                           />
                        </FlexCol>
                        <FlexCol>
                           <TextInput
                              placeholder='Número'
                              defaultValue={itemSelecionado.telefone?.numero}
                              onChange={(value) =>
                                 setItemSelecionado({
                                    telefone: { ...itemSelecionado.telefone, numero: value },
                                 })
                              }
                           />
                        </FlexCol>
                     </FlexRow>
                  </FormGroup>
               </Col>

               <Col>
                  <FormGroup>
                     <BoldLabel>Celular</BoldLabel>
                     <FlexRow gap={4}>
                        <FlexCol width={70}>
                           <TextInput
                              placeholder='DDD'
                              defaultValue={itemSelecionado.celular?.ddd}
                              onChange={(value) =>
                                 setItemSelecionado({
                                    celular: { ...itemSelecionado.celular, ddd: value },
                                 })
                              }
                           />
                        </FlexCol>
                        <FlexCol>
                           <TextInput
                              placeholder='Número'
                              defaultValue={itemSelecionado.celular?.numero}
                              onChange={(value) =>
                                 setItemSelecionado({
                                    celular: { ...itemSelecionado.celular, numero: value },
                                 })
                              }
                           />
                        </FlexCol>
                     </FlexRow>
                  </FormGroup>
               </Col>
               <Col lg={5}>
                  <FormGroup>
                     <BoldLabel>Email</BoldLabel>
                     <EmailInput
                        defaultValue={itemSelecionado.email}
                        onChange={(value) => setItemSelecionado({ email: value })}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col md={4} lg={3} xl={3}>
                  <FormGroup>
                     <BoldLabel>CEP</BoldLabel>
                     <CepInput
                        defaultValue={itemSelecionado.endereco?.endereco?.cep}
                        onChange={(value) =>
                           setItemSelecionado({
                              endereco: {
                                 ...itemSelecionado.endereco,
                                 endereco: { ...itemSelecionado.endereco?.endereco, cep: value },
                              },
                           })
                        }
                     />
                  </FormGroup>
               </Col>

               <Col>
                  <FormGroup>
                     <BoldLabel>Logradouro</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.endereco?.endereco?.logradouro}
                        onChange={(value) =>
                           setItemSelecionado({
                              endereco: {
                                 ...itemSelecionado.endereco,
                                 endereco: { ...itemSelecionado.endereco?.endereco, logradouro: value },
                              },
                           })
                        }
                     />
                  </FormGroup>
               </Col>
               <Col md={2} lg={2} xl={2}>
                  <FormGroup>
                     <BoldLabel>Número</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.endereco?.endereco?.numero}
                        onChange={(value) =>
                           setItemSelecionado({
                              endereco: {
                                 ...itemSelecionado.endereco,
                                 endereco: { ...itemSelecionado.endereco?.endereco, numero: value },
                              },
                           })
                        }
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col md={4} lg={3} xl={3}>
                  <FormGroup>
                     <BoldLabel>Complemento</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.endereco?.endereco?.complemento}
                        onChange={(value) =>
                           setItemSelecionado({
                              endereco: {
                                 ...itemSelecionado.endereco,
                                 endereco: { ...itemSelecionado.endereco?.endereco, complemento: value },
                              },
                           })
                        }
                     />
                  </FormGroup>
               </Col>

               <Col>
                  <FormGroup>
                     <BoldLabel>Bairro</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado.endereco?.endereco?.bairro}
                        onChange={(value) =>
                           setItemSelecionado({
                              endereco: {
                                 ...itemSelecionado.endereco,
                                 endereco: { ...itemSelecionado.endereco?.endereco, bairro: value },
                              },
                           })
                        }
                     />
                  </FormGroup>
               </Col>

               <Col>
                  <FormGroup>
                     <BoldLabel>Cidade</BoldLabel>
                     <Select
                        defaultValue={itemSelecionado.endereco?.endereco?.cidade}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) =>
                           setItemSelecionado({
                              endereco: {
                                 ...itemSelecionado.endereco,
                                 endereco: { ...itemSelecionado.endereco?.endereco, cidade: i },
                              },
                           })
                        }
                        formularioPadrao={(select) => <CidadeView select={select} />}
                        noDropDown
                        readOnlyColor='#ffff'
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col>
                  <FormGroup>
                     <BoldLabel>Observações</BoldLabel>
                     <TextArea
                        defaultValue={itemSelecionado.observacao}
                        rows={3}
                        onChange={(value) => setItemSelecionado({ observacao: value })}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col>
                  <FormGroup>
                     <BoldLabel>Alerta de Saúde</BoldLabel>
                     <TextArea
                        defaultValue={itemSelecionado.alertaDeSaude}
                        rows={3}
                        onChange={(value) => setItemSelecionado({ alertaDeSaude: value })}
                     />
                  </FormGroup>
               </Col>
            </Row>
         </>
      );
   };

   return (
      <Form
         titulo='Leads'
         url={url}
         fastUrl={`${url}/fast`}
         ordenacaoPadrao='dataDeCadastro'
         permissoes={[1051, 1052, 1053, 1054]}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         aposInserir={controller.aposInserir}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         select={props.select}
         itemVazio={controller.itemVazio}
      />
   );
}
