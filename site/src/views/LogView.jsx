import React, { useState, useMemo } from 'react';
import Form from '../components/forms/Form';
import LogController from './LogController';
import { Col, Row, FormGroup } from 'react-bootstrap';
import { dateTimeToString } from '../utils/Functions';
import Label from '../components/Label';
import TextInput from '../components/TextInput';
import TextArea from '../components/TextArea';

export default function LogView(props) {
   const [state, setState] = useState({});

   // memoiza o controller para não recriar em cada render
   const controller = useMemo(
      () =>
         new LogController({
            get state() {
               return state;
            },
            setState,
            props,
         }),
      [state, props]
   );

   const renderizarFormulario = () => {
      const itemSelecionado = state.itemSelecionado || {};
      return (
         <>
            <Row>
               <Col sm={6} md={2} lg={2}>
                  <FormGroup>
                     <Label>{'Código'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.id} />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <Label>{'ID da Requisição'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.groupId} />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <Label>{'Data'}</Label>
                     <TextInput readOnly defaultValue={dateTimeToString(itemSelecionado.date)} />
                  </FormGroup>
               </Col>
               <Col sm={2} md={2} lg={2}>
                  <FormGroup>
                     <Label>{'Usuário'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.userId} />
                  </FormGroup>
               </Col>
               <Col sm={2} md={2} lg={2}>
                  <FormGroup>
                     <Label>{'Categoria'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.subCategory?.description} />
                  </FormGroup>
               </Col>
            </Row>
            <Row>
               <Col>
                  <FormGroup>
                     <Label>{'IP'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.clientAddress} />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <Label>{'Ação'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.action} />
                  </FormGroup>
               </Col>
            </Row>
            <Row>
               <Col>
                  <FormGroup>
                     <Label>{'Método HTTP'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.httpMethod} />
                  </FormGroup>
               </Col>
               <Col>
                  <FormGroup>
                     <Label>{'Resposta HTTP'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.httpResponseCode} />
                  </FormGroup>
               </Col>
            </Row>
            <Row>
               <Col>
                  <FormGroup>
                     <Label>{'Mensagem'}</Label>
                     <TextArea
                        style={{ fontSize: 11 }}
                        readOnly
                        defaultValue={itemSelecionado.message}
                        rows={12}
                     />
                  </FormGroup>
               </Col>
            </Row>
         </>
      );
   };

   const getFormState = () => state;
   const setFormStateWrapper = (newState, callback) =>
      new Promise((resolve) => {
         setState((prev) => ({ ...prev, ...newState }));
         if (callback) callback();
         resolve();
      });

   return (
      <Form
         titulo={'Log'}
         url={'/log'}
         ordenacaoPadrao={'date desc'}
         permissoes={[9001, null, null, null]}
         getFiltro={controller.getFiltro}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         antesDeInserir={controller.antesDeInserir}
         antesDeEditar={controller.antesDeEditar}
         antesDeSalvar={controller.antesDeSalvar}
         antesDeExcluir={controller.antesDeExcluir}
         esconderFiltro={true}
         getFormState={getFormState}
         setFormState={setFormStateWrapper}
         select={props.select}
         itemVazio={{}}
      />
   );
}
