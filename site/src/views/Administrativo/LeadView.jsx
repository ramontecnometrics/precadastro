import React, { useMemo, useRef, useState } from 'react';
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
import { Tab, Tabs } from '../../components/Tabs';
import { ViewController } from '../../components/ViewController';
import Text from '../../components/Text';
import Line from '../../components/Line';
import Button from '../../components/Button';
import { useReactToPrint } from 'react-to-print';
import logo from '../../contents/img/logo.png';
import { calcularIdade, dateToString, formatDate, getEnderecoCompleto } from '../../utils/Functions';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

const url = '/lead';

export default function LeadView(props) {
   const controller = useMemo(() => new LeadController(), []);
   const [aba, setAba] = useState(1);
   const contentRef = useRef(null);
   const handlePrint = useReactToPrint({ contentRef });

   const campos = (itemSelecionado, setItemSelecionado) => {
      return (
         <>
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

   const gerarPDF = async (nomeCompleto) => {
      const el = contentRef.current;

      // Qualidade e espaços
      const SCALE = 2; // qualidade da captura
      const GAP_TOP_MM = 8; // espaço no topo de cada página
      const GAP_BOTTOM_MM = 8; // espaço no rodapé (todas, exceto a última)

      const bigCanvas = await html2canvas(el, {
         scale: SCALE,
         useCORS: true,
         backgroundColor: '#fff',
         scrollX: 0,
         scrollY: -window.scrollY,
      });

      const pdf = new jsPDF('p', 'mm', 'a4');
      const pageW = pdf.internal.pageSize.getWidth();
      const pageH = pdf.internal.pageSize.getHeight();

      // Conversões px <-> mm (considerando que a largura da imagem = pageW)
      const mmPerPx = pageW / bigCanvas.width;
      const pageH_px = Math.floor(pageH / mmPerPx);
      const gapTop_px = Math.round(GAP_TOP_MM / mmPerPx);
      const gapBottom_px = Math.round(GAP_BOTTOM_MM / mmPerPx);

      // Conteúdo útil por página (em px)
      const pageContent_px = pageH_px - gapTop_px - gapBottom_px;
      const pageContentLast_px = pageH_px - gapTop_px; // última página não tem rodapé

      let y_px = 0;

      while (y_px < bigCanvas.height) {
         const remaining_px = bigCanvas.height - y_px;
         const isLast = remaining_px <= pageContentLast_px;

         const sliceH_px = isLast ? remaining_px : Math.min(remaining_px, pageContent_px);

         // Recorta apenas a fatia correspondente ao conteúdo desta página
         const pageCanvas = document.createElement('canvas');
         pageCanvas.width = bigCanvas.width;
         pageCanvas.height = sliceH_px;

         const ctx = pageCanvas.getContext('2d');
         ctx.drawImage(
            bigCanvas,
            0,
            y_px,
            bigCanvas.width,
            sliceH_px, // src
            0,
            0,
            pageCanvas.width,
            sliceH_px // dst
         );

         const imgData = pageCanvas.toDataURL('image/png');
         const imgH_mm = sliceH_px * mmPerPx; // altura mm equivalente do recorte

         // Desenha a fatia respeitando o GAP do topo
         pdf.addImage(imgData, 'PNG', 0, GAP_TOP_MM, pageW, imgH_mm);

         y_px += sliceH_px;
         if (!isLast) pdf.addPage();
      }

      pdf.save(`Avaliação Clínica - ${nomeCompleto}.pdf`);
   };

   const fichaDeAvaliacao = (itemSelecionado, setItemSelecionado) => {
      return (
         <>
            <div ref={contentRef} style={{ color: '#403e3e', padding: 10 }}>
               <div className='show-on-print-onlyxx'>
                  <FlexRow>
                     <FlexCol width={'fit-content'}>
                        <img
                           src={logo}
                           alt='Logo'
                           style={{
                              width: 120,
                           }}
                        />
                     </FlexCol>
                     <FlexCol style={{ paddingTop: 20, paddingLeft: 150 }}>
                        <Text style={{ fontSize: 30, color: 'gray' }}>Avaliação Clínica</Text>
                     </FlexCol>
                  </FlexRow>

                  <Line />

                  <FlexRow>
                     <FlexCol width={'34%'}>
                        <Text>Paciente</Text>
                        <br />
                        <BoldLabel>{itemSelecionado.nomeCompleto}</BoldLabel>
                     </FlexCol>
                     <FlexCol width={'33%'}>
                        <Text>Data de nascimento</Text>
                        <br />
                        <BoldLabel>{`${dateToString(itemSelecionado.dataDeNascimento)} (${calcularIdade(
                           formatDate(itemSelecionado.dataDeNascimento),
                           true
                        )})`}</BoldLabel>
                     </FlexCol>
                     <FlexCol width={'33%'}>
                        <Text>Estado civil</Text>
                        <br />
                        <BoldLabel>{itemSelecionado.estadoCivil?.descricao}</BoldLabel>
                     </FlexCol>
                  </FlexRow>

                  <FlexRow>
                     <FlexCol width={'34%'}>
                        <Text>CPF</Text>
                        <br />
                        <BoldLabel>{itemSelecionado.cpf}</BoldLabel>
                     </FlexCol>
                     <FlexCol width={'33%'}>
                        <Text>Celular</Text>
                        <br />
                        <BoldLabel>{itemSelecionado.celular.numeroComDDD}</BoldLabel>
                     </FlexCol>
                     <FlexCol width={'33%'}>
                        <Text>E-mail</Text>
                        <br />
                        <BoldLabel>{itemSelecionado.email}</BoldLabel>
                     </FlexCol>
                  </FlexRow>

                  <FlexRow>
                     <FlexCol width={'34%'}>
                        <Text>Ocupação profissional</Text>
                        <br />
                        <BoldLabel>{itemSelecionado.profissao?.nome}</BoldLabel>
                     </FlexCol>
                     <FlexCol width={'66%'}>
                        <Text>Endereço</Text>
                        <br />
                        <BoldLabel>{getEnderecoCompleto(itemSelecionado.endereco.endereco)}</BoldLabel>
                     </FlexCol>
                  </FlexRow>
               </div>

               {itemSelecionado &&
                  itemSelecionado.avaliacaoClinica &&
                  itemSelecionado.avaliacaoClinica.grupos &&
                  itemSelecionado.avaliacaoClinica.grupos.map((grupo, grupoIndex) => {
                     return grupo.campos.map((campo, campoIndex) => {
                        const color = campoIndex % 2 === 0 ? '#d4af3729' : '#ffffff';
                        return (
                           <React.Fragment key={`grupo-${grupoIndex}-campo-${campoIndex}`}>
                              <Line />
                              <FlexRow
                                 className={'no-break-on-print page-break'}
                                 style={{
                                    backgroundColor: color,
                                    paddingLeft: 5,
                                    paddingRight: 5,
                                    paddingTop: 5,
                                    borderLeft: '1px solid #ced4da',
                                    borderRight: '1px solid #ced4da',
                                 }}
                              >
                                 <FlexCol>
                                    <BoldLabel>{campo.titulo}</BoldLabel>
                                 </FlexCol>
                                 <FlexCol>
                                    <Text>{campo.valor}</Text>
                                 </FlexCol>
                              </FlexRow>
                           </React.Fragment>
                        );
                     });
                  })}
               {/* 
               {itemSelecionado &&
                  itemSelecionado.avaliacaoClinica &&
                  itemSelecionado.avaliacaoClinica.grupos &&
                  itemSelecionado.avaliacaoClinica.grupos.map((grupo, grupoIndex) => {
                     return grupo.campos.map((campo, campoIndex) => {
                        const color = campoIndex % 2 === 0 ? '#d4af3729' : '#ffffff';
                        return (
                           <React.Fragment key={`grupo-${grupoIndex}-campo-${campoIndex}`}>
                              <Line />
                              <FlexRow
                                 className={'no-break-on-print page-break'}
                                 style={{
                                    backgroundColor: color,
                                    paddingLeft: 5,
                                    paddingRight: 5,
                                    paddingTop: 5,
                                    borderLeft: '1px solid #ced4da',
                                    borderRight: '1px solid #ced4da',
                                 }}
                              >
                                 <FlexCol>
                                    <BoldLabel>{campo.titulo}</BoldLabel>
                                 </FlexCol>
                                 <FlexCol>
                                    <Text>{campo.valor}</Text>
                                 </FlexCol>
                              </FlexRow>
                           </React.Fragment>
                        );
                     });
                  })} */}
               <Line />
            </div>
            <Filler height={10} />
            <FlexRow>
               <FlexCol width={'fit-content'}>
                  <Button style={{ width: 100 }} text={'Imprimir'} onClick={handlePrint} />
               </FlexCol>
               <FlexCol width={'fit-content'}>
                  <Button style={{ width: 100 }} text={'PDF'} onClick={() => gerarPDF(itemSelecionado.nomeCompleto)} />
               </FlexCol>
               {/* <FlexCol style={{ paddingLeft: 8 }}>
                  <Button style={{ width: 100 }} text={'Alterar'} />
               </FlexCol> */}
            </FlexRow>
            <Filler height={10} />
         </>
      );
   };

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

            <Filler height={8} />
            <Tabs
               activeKey={aba}
               defaultActiveKey='1'
               transition={false}
               id='noanim-tab-example'
               className='mb-3'
               onSelect={(k) => setAba(k)}
            >
               <Tab eventKey='1' title='Cadastro'></Tab>
               <Tab eventKey='2' title='Avaliação clínica'></Tab>
            </Tabs>

            <ViewController
               selected={parseInt(aba)}
               v1={campos(itemSelecionado, setItemSelecionado)}
               v2={fichaDeAvaliacao(itemSelecionado, setItemSelecionado)}
            />
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
