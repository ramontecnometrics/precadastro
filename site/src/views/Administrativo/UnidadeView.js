import React, { useMemo, useRef } from 'react';
import Form from '../../components/forms/Form';
import Label from '../../components/Label';
import TextInput from '../../components/TextInput';
import UnidadeController from './UnidadeController';
import PasswordInput from '../../components/PasswordInput';
import { FlexRow, FlexCol } from '../../components/FlexItems';
import FormGroup from '../../components/FormGroup';
import { Row, Col } from '../../components/Grid';
import BoldLabel from '../../components/BoldLabel';
import Panel from '../../components/Panel';
import QRCode from 'react-qr-code';
import { LayoutParams } from '../../config/LayoutParams';
import Filler from '../../components/Filler';
import Button from '../../components/Button';
import { useReactToPrint } from 'react-to-print';

export default function UnidadeView(props) {
   const lastFormStateRef = useRef(null);
   const controller = useMemo(() => new UnidadeController(), []);
   const contentRef = useRef<HTMLDivElement>(null);
   const handlePrint = useReactToPrint({ contentRef });

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
                     />
                  </FormGroup>
               </Col>
            </Row>
            <Row>
               <Col>
                  <BoldLabel>Integração com Uno</BoldLabel>
               </Col>
            </Row>
            <Row>
               <Col>
                  <FormGroup>
                     <Label>Código de acesso</Label>
                     <PasswordInput
                        defaultValue={item.unoAccessToken || ''}
                        rows={props.rows || 15}
                        onChange={(value) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: {
                                 ...prev.itemSelecionado,
                                 unoAccessToken: value,
                              },
                           }))
                        }
                     />
                  </FormGroup>
               </Col>

               <Col>
                  <FormGroup>
                     <Label>Chave secreta</Label>

                     <PasswordInput
                        defaultValue={item.unoSecretKey || ''}
                        rows={props.rows || 15}
                        onChange={(value) =>
                           setFormState((prev) => ({
                              ...prev,
                              itemSelecionado: {
                                 ...prev.itemSelecionado,
                                 unoSecretKey: value,
                              },
                           }))
                        }
                     />
                  </FormGroup>
               </Col>
            </Row>
            <br />

            <Panel style={{ backgroundColor: '#f8f9fa', padding: 10 }}>
               <div ref={contentRef}>
                  {/* <BoldLabel>QR Code para acesso ao Pré-cadastro</BoldLabel>
                  <FlexRow>
                     <FlexCol style={{ width: 35 }}>
                        <BoldLabel>Link:</BoldLabel>
                     </FlexCol>
                     <FlexCol>
                        <a
                           href={`${window.location.origin}?unidade=${item.id}`}
                           style={{ color: 'blue' }}
                           target='_blank'
                        >{`${window.location.origin}?unidade=${item.id}`}</a>
                     </FlexCol>
                  </FlexRow>
                  <Filler height={10} />
                  <div style={{ height: 'auto', margin: '0 auto', maxWidth: 300, width: '100%' }}>
                     <QRCode
                        size={256}
                        style={{ height: 'auto', maxWidth: '100%', width: '100%' }}
                        value={item.id}
                        viewBox={`0 0 256 256`}
                     />
                  </div>
                  <Filler height={10} /> */}
               </div>
               {/* <Button text={'Imprimir'} style={{ width: 120 }} onClick={handlePrint} /> */}
               <button onClick={handlePrint}>Print</button>
               <Filler height={10} />
            </Panel>
         </>
      );
   };

   return (
      <Form
         titulo='Unidades'
         url='/unidade'
         ordenacaoPadrao='id'
         permissoes={[1071, 1072, 1073, 1074]}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         select={props.select}
         itemVazio={controller.itemVazio}
      />
   );
}
