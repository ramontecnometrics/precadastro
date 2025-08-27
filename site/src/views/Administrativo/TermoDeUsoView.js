import React, { useMemo, useRef } from 'react';
import { Row, Col, FormGroup } from 'react-bootstrap';
import Form from '../../components/forms/Form';
import Label from '../../components/Label';
import TextInput from '../../components/TextInput';
import TextArea from '../../components/TextArea';
import TermoDeUsoController from './TermoDeUsoController';

export default function TermoDeUsoView(props) {
   const lastFormStateRef = useRef(null);
   const controller = useMemo(() => new TermoDeUsoController(), []);

   const renderizarFormulario = ({ formState, setFormState }) => {
      lastFormStateRef.current = formState;

      const item = formState.itemSelecionado || controller.itemVazio;

      return (
         <>
            <Row>
               <Col sm={3} md={3} lg={3}>
                  <FormGroup>
                     <Label>Código</Label>
                     <TextInput readOnly defaultValue={item.id || ''} />
                  </FormGroup>
               </Col>
               <Col sm={9} md={9} lg={9}>
                  <FormGroup>
                     <Label>Nome</Label>
                     <TextInput
                        defaultValue={item.nome}
                        onChange={(value) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: {
                                 ...prev.itemSelecionado,
                                 nome: value,
                              },
                           }))
                        }
                        upperCase
                        readOnly={!!item.id}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               <Col>
                  <FormGroup>
                     <Label>Termo</Label>
                     <TextArea
                        defaultValue={item.termo || ''}
                        rows={props.rows || 15}
                        onChange={(value) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: {
                                 ...prev.itemSelecionado,
                                 termo: value,
                              },
                           }))
                        }
                        readOnly={!!item.id}
                     />
                  </FormGroup>
               </Col>
            </Row>
            <br />
         </>
      );
   };

   return (
      <Form
         titulo='Termos de Uso'
         url='/termodeuso'
         ordenacaoPadrao='id'
         permissoes={[1381, 1382, null, null]}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         select={props.select}
         itemVazio={controller.itemVazio}
      />
   );
}
