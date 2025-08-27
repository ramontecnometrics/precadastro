import React, { useState, useEffect, useMemo, useRef } from 'react';
import Form from '../components/forms/Form';
import AuditoriaController from './AuditoriaController';
import { Col, Row, FormGroup } from 'react-bootstrap';
import { buildQueryString, dateTimeToString } from '../utils/Functions';
import Label from '../components/Label';
import TextInput from '../components/TextInput';
import TextArea from '../components/TextArea';
import Button from '../components/Button';
import Select from '../components/Select';
import UsuarioAdministradorView from './Administrativo/UsuarioAdministradorView';
import DateInput from '../components/DateInput';
import TimeInput from '../components/TimeInput';
import sessionManager from '../SessionManager';
import api from '../utils/Api';

export default function AuditoriaView(props) {
   const [state, setState] = useState({
      acoes: [],
      dataInicial: new Date(),
      horaInicial: '00:00',
      dataFinal: new Date(),
      horaFinal: '23:59',
   });

   const formRef = useRef(null);

   const isTecnometrics = sessionManager.isUsuarioTecnometrics();

   // cria controller apenas uma vez
   const controller = useMemo(
      () =>
         new AuditoriaController({
            get state() {
               return state;
            },
            setState,
            props,
         }),
      [state, props]
   );

   // carregar ações ao montar
   useEffect(() => {
      api.get('/auditoria/acoes').then((result) => {
         setState((prev) => ({ ...prev, acoes: result }));
      });
   }, []);

   const renderizarFormulario = () => {
      const itemSelecionado = state.itemSelecionado || {};
      return (
         <>
            <Row>
               <Col sm={4} md={4} lg={2}>
                  <FormGroup>
                     <Label>{'Código'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.id} />
                  </FormGroup>
               </Col>
               <Col sm={8} md={8} lg={3}>
                  <FormGroup>
                     <Label>{'Data'}</Label>
                     <TextInput readOnly defaultValue={dateTimeToString(itemSelecionado.data)} />
                  </FormGroup>
               </Col>
               <Col sm={6} md={6} lg={3}>
                  <FormGroup>
                     <Label>{'Usuário'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.nomeDoUsuario} />
                  </FormGroup>
               </Col>
               <Col sm={6} md={6} lg={4}>
                  <FormGroup>
                     <Label>{'Ação'}</Label>
                     <TextInput readOnly defaultValue={itemSelecionado.acao?.descricao} />
                  </FormGroup>
               </Col>
            </Row>
            <Row>
               <Col>
                  <FormGroup>
                     <Label>{'Detalhes'}</Label>
                     <TextArea style={{ fontSize: 11 }} readOnly defaultValue={itemSelecionado.descricao} rows={12} />
                  </FormGroup>
               </Col>
            </Row>
         </>
      );
   };

   const renderizarFiltros = () => {
      return (
         <div>
            <Row>
               {isTecnometrics && (
                  <Col sm={6} md={6} lg={2}>
                     <FormGroup>
                        <Label>Usuário</Label>
                        <Select
                           name={'usuario'}
                           getKeyValue={(i) => i.id}
                           getDescription={(i) => i.nomeDeUsuario}
                           onSelect={(i) => setState((prev) => ({ ...prev, usuario: i }))}
                           formularioPadrao={(select) => <UsuarioAdministradorView select={select} />}
                           noDropDown={true}
                           readOnlyColor='#ffff'
                           getFilterUrl={(text) =>
                              '/usuarioadministrador/fast' +
                              buildQueryString(2, null, 'id', { Searchable: text })
                           }
                        />
                     </FormGroup>
                  </Col>
               )}

               <Col sm={6} md={6} lg={2}>
                  <FormGroup>
                     <Label>Ação</Label>
                     {state.acoes && (
                        <Select
                           name={'acao'}
                           options={state.acoes}
                           getKeyValue={(i) => i.id}
                           getDescription={(i) => i.descricao}
                           onSelect={(i) => setState((prev) => ({ ...prev, acao: i }))}
                           nullText={''}
                        />
                     )}
                  </FormGroup>
               </Col>

               <Col sm={6} md={3} lg={2}>
                  <FormGroup>
                     <Label>{'Data inicial'}</Label>
                     <DateInput
                        defaultValue={state.dataInicial}
                        onChange={(value) => setState((prev) => ({ ...prev, dataInicial: value }))}
                     />
                  </FormGroup>
               </Col>

               <Col sm={6} md={2} lg={1}>
                  <FormGroup>
                     <Label>{'Hora'}</Label>
                     <TimeInput
                        defaultValue={state.horaInicial}
                        onChange={(value) => setState((prev) => ({ ...prev, horaInicial: value }))}
                     />
                  </FormGroup>
               </Col>

               <Col sm={6} md={3} lg={2}>
                  <FormGroup>
                     <Label>{'Data final'}</Label>
                     <DateInput
                        defaultValue={state.dataFinal}
                        onChange={(value) => setState((prev) => ({ ...prev, dataFinal: value }))}
                     />
                  </FormGroup>
               </Col>

               <Col sm={6} md={2} lg={1}>
                  <FormGroup>
                     <Label>{'Hora'}</Label>
                     <TimeInput
                        defaultValue={state.horaFinal}
                        onChange={(value) => setState((prev) => ({ ...prev, horaFinal: value }))}
                     />
                  </FormGroup>
               </Col>

               <Col sm={6} md={2} lg={2}>
                  <FormGroup>
                     <Label>&nbsp;</Label>
                     <Button text={'Filtrar'} onClick={() => formRef.current?.filtrar()} />
                  </FormGroup>
               </Col>
            </Row>
         </div>
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
         titulo={'Auditoria'}
         url={'/auditoria'}
         ordenacaoPadrao={'id desc'}
         ref={formRef}
         permissoes={[1501, null, null, null]}
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
         renderizarFiltros={renderizarFiltros}
         getFormState={getFormState}
         setFormState={setFormStateWrapper}
         select={props.select}
         itemVazio={{}}
      />
   );
}
