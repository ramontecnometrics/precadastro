import React, { useMemo, useRef } from "react";
import { Row, Col, FormGroup } from "react-bootstrap";
import Form , {makeFormHelpers} from "../components/forms/Form";
import Label from "../components/Label";
import TextInput from "../components/TextInput";
import RotinaDoSistemaController from "./RotinaDoSistemaController"; 

export default function RotinaDoSistemaView(props) {
   const lastFormStateRef = useRef(null);

   const controller = useMemo(() => new RotinaDoSistemaController(), []);

   const renderizarFormulario = ({ formState, setFormState }) => {
    const { setItemSelecionado } = makeFormHelpers(setFormState);

      lastFormStateRef.current = formState;

      const itemSelecionado = formState.itemSelecionado;

      return (
      <>
         <Row>
            <Col sm={3}>
               <FormGroup>
                  <Label>Código</Label>
                  <TextInput readOnly defaultValue={itemSelecionado.id || ""} />
               </FormGroup>
            </Col>

            <Col sm={9}>
               <FormGroup>
                  <Label>Descrição</Label>
                  <TextInput
                     defaultValue={itemSelecionado.descricao || ""}
                     onChange={(value) => setItemSelecionado({ descricao: value })}
                     onInput={(value) => setItemSelecionado({ descricao: value })}
                     readOnly
                  />
               </FormGroup>
            </Col>
         </Row>
         <br />
         <br />
      </>
      );
   };

   return (
      <Form
         titulo="Rotinas do Sistema"
         url="/rotinadosistema"
         ordenacaoPadrao="id"
         esconderTitulo={false}
         permissoes={[1031, null, null, null]}
         filtroExtra={props.filtroExtra}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         select={props.select}
         itemVazio={{}}         
      />
   );
}
