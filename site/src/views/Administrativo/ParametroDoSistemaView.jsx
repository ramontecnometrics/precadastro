import React, { useMemo, useRef } from 'react';
import { FormGroup } from 'react-bootstrap';
import Form from '../../components/forms/Form';
import Label from '../../components/Label';
import TextInput from '../../components/TextInput';
import Text from '../../components/Text';
import Select from '../../components/Select';
import PasswordInput from '../../components/PasswordInput';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import ParametroDoSistemaController from './ParametroDoSistemaController';

export default function ParametroDoSistemaView(props) {
  // refs para expor o formState/setFormState atuais ao controller
  const lastFormStateRef = useRef(null);
  const setFormStateRef = useRef(null);

  // controller com acesso ao formState do Form
  const controller = useMemo(
    () =>
      new ParametroDoSistemaController({
        getFormState: () => lastFormStateRef.current,
        setFormState: (patchOrFn) => {
          if (!setFormStateRef.current) return;
          setFormStateRef.current((prev) =>
            typeof patchOrFn === 'function' ? patchOrFn(prev) : { ...prev, ...patchOrFn }
          );
        },
      }),
    []
  );

  const comboParametros = useMemo(() => controller.comboParametros(), [controller]);

  const antesDeEditar = async (_sender, item) => {
    // Guarda o tipo de controle no formState para ser usado no render
    setFormStateRef.current?.((prev) => ({
      ...prev,
      tipoDeControle: item?.protegido ? 'password' : 'text',
    }));
  };

  const renderizarFormulario = ({ formState, setFormState }) => {
    // expõe para o controller
    lastFormStateRef.current = formState;
    setFormStateRef.current = setFormState;

    const item = formState.itemSelecionado || {};
    const tipoDeControle = formState.tipoDeControle || (item?.protegido ? 'password' : 'text');

    const parametro =
      item?.nome ? comboParametros.find((i) => i.nome === item.nome) : null;

    const setItem = (patch) =>
      setFormState((prev) => ({
        ...prev,
        itemSelecionado: { ...prev.itemSelecionado, ...patch },
      }));

    return (
      <>
        <FormGroup>
          <Label>Parâmetro</Label>
          <Select
            options={comboParametros}
            name="parametro"
            defaultValue={item?.nome || ''}
            getKeyValue={(i) => i.nome}
            getDescription={(i) => i.descricao}
            onSelect={(i) => {
              if (i) {
                setFormState((prev) => ({
                  ...prev,
                  itemSelecionado: {
                    ...prev.itemSelecionado,
                    nome: i.nome,
                    descricao: i.descricao,
                    protegido: i.protegido,
                    grupo: i.grupo,
                    ordem: i.ordem,
                    componente: i.componente,
                  },
                  tipoDeControle: i.protegido ? 'password' : 'text',
                }));
              } else {
                setFormState((prev) => ({
                  ...prev,
                  itemSelecionado: {
                    ...prev.itemSelecionado,
                    nome: null,
                    descricao: null,
                    protegido: null,
                    grupo: null,
                    ordem: null,
                    componente: null,
                  },
                  tipoDeControle: 'text',
                }));
              }
            }}
          />
        </FormGroup>

        {parametro && !parametro.componente && tipoDeControle !== 'password' && (
          <FormGroup>
            <Label>{parametro.descricao}</Label>
            <TextInput
              type="text"
              defaultValue={item?.valor ?? ''}
              onChange={(value) => setItem({ valor:  value })}
            />
          </FormGroup>
        )}

        {parametro && !parametro.componente && tipoDeControle === 'password' && (
          <FormGroup>
            <Label>{parametro.descricao}</Label>
            <PasswordInput
              defaultValue={item?.valor ?? ''}
              onChange={(value) => setItem({ valor: value })}
              placeholder={item?.preenchido ? 'Sobrescrever o valor atual' : null}
            />
          </FormGroup>
        )}

        {parametro?.componente && (
          <div>
            {parametro.componente(item?.valor, (valor) => setItem({ valor }))}
          </div>
        )}

        {parametro?.ajuda && (
          <div style={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
            <div style={{ display: 'table-cell', fontSize: 22, color: '#0062cc' }}>
              <FontAwesomeIcon icon={faInfoCircle} />
            </div>
            <div style={{ display: 'table-cell', fontSize: 14, paddingTop: 5, paddingLeft: 3 }}>
              <Text style={{ whiteSpace: 'pre-wrap' }}>{parametro.ajuda}</Text>
            </div>
          </div>
        )}
      </>
    );
  };

  return (
    <Form
      titulo="Parâmetros do Sistema"
      url="/parametrodosistema"
      protected
      ordenacaoPadrao="grupo, ordem"
      permissoes={[1041, 1042, 1043, 1044]}
      antesDeEditar={antesDeEditar}
      getFiltro={controller.getFiltro}
      getTitulosDaTabela={controller.getTitulosDaTabela}
      getDadosDaTabela={controller.getDadosDaTabela}
      renderizarFormulario={renderizarFormulario}
      getObjetoDeDados={() => controller.getObjetoDeDados(lastFormStateRef.current)}
      select={props.select}
      itemVazio={{
        // inicia com estrutura vazia “segura” para controlar inputs
        nome: '',
        descricao: '',
        protegido: false,
        valor: '',
        grupo: '',
        ordem: '',
        componente: null,
        preenchido: false,
      }}
    />
  );
}
