import React, { useMemo, useRef } from 'react';
import Row from '../../components/Row';
import Col from '../../components/Col';
import FormGroup from '../../components/FormGroup';
import Form from '../../components/forms/Form';
import Select from '../../components/Select';
import CheckBox from '../../components/CheckBox'; 
import TextInput from '../../components/TextInput';
import PerfilDeUsuarioController from './PerfilDeUsuarioController';
import FormDetails from '../../components/forms/FormDetails';
import Filler from '../../components/Filler';
import BoldLabel from '../../components/BoldLabel';

export default function PerfilDeUsuarioView(props) {
   const controller = useMemo(() => new PerfilDeUsuarioController(), []);
   const lastFormStateRef = useRef(null);

   const renderizarFormulario = ({ formState, setFormState }) => {
      lastFormStateRef.current = formState;

      const itemSelecionado = formState.itemSelecionado || { situacao: { id: 1 }, acessos: [] };
      const permitirTudo = !!formState.permitirTudo;

      return (
         <>
            <Row>
               <Col sm={3}>
                  <FormGroup>
                     <BoldLabel>Código</BoldLabel>
                     <TextInput readOnly defaultValue={itemSelecionado?.id || ''} />
                  </FormGroup>
               </Col>

               <Col sm={9}>
                  <FormGroup>
                     <BoldLabel>Situação</BoldLabel>
                     <Select
                        as='select'
                        name='situacao'
                        defaultValue={itemSelecionado?.situacao?.id || ''}
                        options={[
                           { id: 1, descricao: 'Ativo' },
                           { id: 2, descricao: 'Inativo' },
                        ]}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.descricao}
                        onSelect={(i) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: { ...prev.itemSelecionado, situacao: i },
                           }))
                        }
                        disabled={itemSelecionado?.nome?.toUpperCase() === 'MASTER'}
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Row>
               {/* <Col sm={3}>
                  <FormGroup>
                     <Label>Tipo de Perfil</Label>
                     <Select
                        as='select'
                        name='tipoDePerfil'
                        defaultValue={itemSelecionado?.tipoDePerfil?.id || ''}
                        options={
                           itemSelecionado?.nome?.toUpperCase() === 'MASTER'
                              ? [{ id: 1, descricao: 'Administrador' }]
                              : [{ id: 1, descricao: 'Administrador' }]
                        }
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.descricao}
                        onSelect={(i) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: { ...prev.itemSelecionado, tipoDePerfil: i, acessos: [] },
                           }))
                        }
                        disabled={itemSelecionado?.nome?.toUpperCase() === 'MASTER'}
                     />
                  </FormGroup>
               </Col> */}

               <Col sm={12}>
                  <FormGroup>
                     <BoldLabel>Nome</BoldLabel>
                     <TextInput
                        defaultValue={itemSelecionado?.nome || ''}
                        onChange={(value) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: { ...prev.itemSelecionado, nome: value },
                           }))
                        }
                        readOnly={itemSelecionado?.nome?.toUpperCase() === 'MASTER'}
                        upperCase
                     />
                  </FormGroup>
               </Col>
            </Row>

            <Filler height={12} />

            {itemSelecionado?.tipoDePerfil &&
               itemSelecionado.tipoDePerfil.id !== 0 &&
               itemSelecionado?.nome?.toUpperCase() !== 'MASTER' && (
                  <FormDetails
                     titulo='Acessos'
                     itens={itemSelecionado.acessos}
                     tituloDasAcoes={() => (
                        <div style={{ maxWidth: 26, margin: 'auto', paddingTop: 3, paddingLeft: 5 }}>
                           <CheckBox
                              checked={permitirTudo}
                              onChange={(checked) => {
                                 const novosAcessos = (itemSelecionado.acessos || []).map((a) => ({
                                    ...a,
                                    acessoPermitido: checked,
                                 }));
                                 setFormState((prev) => ({
                                    ...prev,
                                    permitirTudo: checked,
                                    itemSelecionado: { ...prev.itemSelecionado, acessos: novosAcessos },
                                 }));
                              }}
                           />
                        </div>
                     )}
                     colunas={() => [
                        { titulo: 'Código', width: '20%' },
                        { titulo: 'Descrição', width: '80%' },
                     ]}
                     linha={(item) => [item.rotina.id, item.rotina.descricao]}
                     acoes={(sender, index) => {
                        const acesso = itemSelecionado.acessos[index];
                        return (
                           <CheckBox
                              checked={!!acesso.acessoPermitido}
                              onChange={(checked) => {
                                 const novosAcessos = [...(itemSelecionado.acessos || [])];
                                 novosAcessos[index] = { ...acesso, acessoPermitido: checked };
                                 setFormState((prev) => ({
                                    ...prev,
                                    itemSelecionado: { ...prev.itemSelecionado, acessos: novosAcessos },
                                 }));
                              }}
                           />
                        );
                     }}
                  />
               )}
            <br />
         </>
      );
   };

   return (
      <Form
         titulo='Perfil de Usuário'
         url='/perfildeusuario'
         fastUrl='/perfildeusuario/fast'
         ordenacaoPadrao='nome'
         permissoes={[1011, 1012, 1013, 1014]}
         getFiltro={controller.getFiltro}
         filtroExtra={props.filtroExtra}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         // Passa o formState atual pro controller no salvar
         getObjetoDeDados={() => controller.getObjetoDeDados(lastFormStateRef.current)}
         antesDeInserir={controller.antesDeInserir}
         antesDeEditar={controller.antesDeEditar}
         antesDeSalvar={controller.antesDeSalvar}
         antesDeExcluir={controller.antesDeExcluir}
         aposInserir={controller.aposInserir}
         select={props.select}
         itemVazio={controller.itemVazio}
      />
   );
}
