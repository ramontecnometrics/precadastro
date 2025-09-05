import React, { useMemo, useRef } from 'react';
import Form from '../../components/forms/Form';
import TextInput from '../../components/TextInput';
import UnidadeController from './UnidadeController';
import PasswordInput from '../../components/PasswordInput';
import { FlexRow, FlexCol } from '../../components/FlexItems';
import FormGroup from '../../components/FormGroup';
import { Row, Col } from '../../components/Grid';
import BoldLabel from '../../components/BoldLabel';
import Panel from '../../components/Panel';
import QRCode from 'react-qr-code';
import Filler from '../../components/Filler';
import Button from '../../components/Button';
import { useReactToPrint } from 'react-to-print';
import logo from '../../contents/img/logo.svg';
import Text from '../../components/Text';
import Line from '../../components/Line';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import IconButton from '../../components/IconButton';

export default function UnidadeView(props) {
   const lastFormStateRef = useRef(null);
   const controller = useMemo(() => new UnidadeController(), []);
   const contentPreCadastroRef = useRef(null);
   const contentAnamneseRef = useRef(null);
   const handlePrintPreCadastro = useReactToPrint({ contentRef: contentPreCadastroRef });
   const handlePrintAnamnese = useReactToPrint({ contentRef: contentAnamneseRef });

   const renderizarFormulario = ({ formState, setFormState }) => {
      lastFormStateRef.current = formState;

      const item = formState.itemSelecionado;

      const getPreCadastroLink = () => `${window.location.origin}/cadastro?id=${item.uuid}`;
      const getLinkAnamnese = () => `${window.location.origin}/anamnese?id=${item.uuid}`;

      return (
         <>
            <Row>
               <Col sm={3} md={3} lg={3}>
                  <FormGroup>
                     <BoldLabel>Código</BoldLabel>
                     <TextInput readOnly defaultValue={item.id || ''} />
                  </FormGroup>
               </Col>
               <Col sm={9} md={9} lg={9}>
                  <FormGroup>
                     <BoldLabel>Nome</BoldLabel>
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
            <Filler height={10} />
            <Panel style={{ backgroundColor: '#f8f9fa', padding: 10 }}>
               <Row>
                  <Col>
                     <BoldLabel>Integração com Uno</BoldLabel>
                  </Col>
               </Row>
               <Row>
                  <Col>
                     <FormGroup>
                        <BoldLabel>Código de acesso</BoldLabel>
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
                        <BoldLabel>Chave secreta</BoldLabel>

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
            </Panel>
            <br />

            {item.uuid && (
               <>
                  <Panel style={{ backgroundColor: '#f8f9fa', padding: 10 }}>
                     <div>
                        <BoldLabel>QR Code para acesso ao Pré-cadastro</BoldLabel>
                        <FlexRow>
                           <FlexCol style={{ width: 35 }}>
                              <BoldLabel>Link:</BoldLabel>
                           </FlexCol>
                           <FlexCol style={{ paddingLeft: 3 }}>
                              <a href={getPreCadastroLink()} style={{ color: 'blue' }} target='_blank'>
                                 {getPreCadastroLink()}
                              </a>
                              <IconButton
                                 title='Copiar link'
                                 style={{ marginLeft: 10, fontSize: 18, color: 'gray' }}
                                 icon={faCopy}
                                 onClick={() => navigator.clipboard.writeText(getPreCadastroLink())}
                              />
                           </FlexCol>
                        </FlexRow>
                        <Filler height={10} />
                        <div style={{ height: 'auto', margin: '0 auto', maxWidth: 300, width: '100%' }}>
                           <QRCode
                              size={256}
                              style={{ height: 'auto', maxWidth: '100%', width: '100%' }}
                              value={getPreCadastroLink()}
                              viewBox={`0 0 256 256`}
                           />
                        </div>
                        <Filler height={10} />
                     </div>

                     <div ref={contentPreCadastroRef}>
                        <div className='show-on-print-only' style={{ textAlign: 'center' }}>
                           <div style={{ height: 250, overflowY: 'clip' }}>
                              <img
                                 src={logo}
                                 alt='Logo'
                                 style={{
                                    width: 300,
                                 }}
                              />
                           </div>

                           <Line />

                           <Filler height={30} />
                           <Text style={{ fontSize: 32, color: 'gray' }}>Formulário de Pré-cadastro</Text>
                           <Filler height={40} />
                           <div style={{ height: 'auto', margin: '0 auto', maxWidth: 300, width: '100%' }}>
                              <QRCode
                                 size={256}
                                 style={{ height: 'auto', maxWidth: '100%', width: '100%' }}
                                 value={getPreCadastroLink()}
                                 viewBox={`0 0 256 256`}
                              />
                           </div>
                           <Filler height={40} />
                           <BoldLabel>{getPreCadastroLink()}</BoldLabel>
                           <Filler height={20} />
                           <Text style={{ fontSize: 32, color: 'gray' }}>{item.nome}</Text>
                        </div>
                     </div>

                     <Button text={'Imprimir'} style={{ width: 120 }} onClick={handlePrintPreCadastro} />
                     <Filler height={10} />
                  </Panel>
                  <br />

                  <Panel style={{ backgroundColor: '#f8f9fa', padding: 10 }}>
                     <div>
                        <BoldLabel>QR Code para realizar a Anamnese</BoldLabel>
                        <FlexRow>
                           <FlexCol style={{ width: 35 }}>
                              <BoldLabel>Link:</BoldLabel>
                           </FlexCol>
                           <FlexCol style={{ paddingLeft: 3 }}>
                              <a href={getLinkAnamnese()} style={{ color: 'blue' }} target='_blank'>
                                 {getLinkAnamnese()}
                              </a>
                              <IconButton
                                 title='Copiar link'
                                 style={{ marginLeft: 10, fontSize: 18, color: 'gray' }}
                                 icon={faCopy}
                                 onClick={() => navigator.clipboard.writeText(getLinkAnamnese())}
                              />
                           </FlexCol>
                        </FlexRow>
                        <Filler height={10} />
                        <div style={{ height: 'auto', margin: '0 auto', maxWidth: 300, width: '100%' }}>
                           <QRCode
                              size={256}
                              style={{ height: 'auto', maxWidth: '100%', width: '100%' }}
                              value={getLinkAnamnese()}
                              viewBox={`0 0 256 256`}
                           />
                        </div>
                        <Filler height={10} />
                     </div>

                     <div ref={contentAnamneseRef}>
                        <div className='show-on-print-only' style={{ textAlign: 'center' }}>
                           <div style={{ height: 250, overflowY: 'clip' }}>
                              <img
                                 src={logo}
                                 alt='Logo'
                                 style={{
                                    width: 300,
                                 }}
                              />
                           </div>

                           <Line />

                           <Filler height={30} />
                           <Text style={{ fontSize: 32, color: 'gray' }}>Formulário de Anamnese</Text>
                           <Filler height={40} />
                           <div style={{ height: 'auto', margin: '0 auto', maxWidth: 300, width: '100%' }}>
                              <QRCode
                                 size={256}
                                 style={{ height: 'auto', maxWidth: '100%', width: '100%' }}
                                 value={getLinkAnamnese()}
                                 viewBox={`0 0 256 256`}
                              />
                           </div>
                           <Filler height={40} />
                           <BoldLabel>{getLinkAnamnese()}</BoldLabel>
                           <Filler height={20} />
                           <Text style={{ fontSize: 32, color: 'gray' }}>{item.nome}</Text>
                        </div>
                     </div>

                     <Button text={'Imprimir'} style={{ width: 120 }} onClick={handlePrintAnamnese} />
                     <Filler height={10} />
                  </Panel>

                  <br />
                  <br />
               </>
            )}
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
         aposInserir={controller.aposInserir}
         select={props.select}
         itemVazio={controller.itemVazio}
      />
   );
}
