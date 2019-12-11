using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using CG_Biblioteca;
using System.Drawing;

namespace textura
{
  internal abstract class Objeto
  {
    protected string rotulo;
    protected PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    private float primitivaTamanho = 2;
    protected BBox bBox = new BBox();

    protected Objeto objetoPai = null;

    protected Color cor = Color.Yellow;
    public List<Objeto> objetosLista = new List<Objeto>();

     protected Transformacao4D matriz = new Transformacao4D();
    /// Matrizes temporarias que sempre sao inicializadas com matriz Identidade entao podem ser "static".
    private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
    private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
    private static Transformacao4D matrizTmpEscala = new Transformacao4D();
    private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
    protected static Transformacao4D matrizGlobal = new Transformacao4D();

    protected Transformacao4D matrizMundo = new Transformacao4D();

    public void setMatrizMundo(Transformacao4D matriz){
      this.matrizMundo = matriz;
      this.atualizarBbox();
    }

    public Transformacao4D getMatrizMundo(){
      return this.matrizMundo;
    }
    
    public void setObjetoPai(Objeto pai){
      this.objetoPai = pai;
    }

    public Objeto getObjetoPai(){
      return this.objetoPai;
    }

    public Transformacao4D GetMatriz(){
      return this.matriz.MultiplicarMatriz(this.matrizMundo);
    }

    protected void atualizarMatrizFilhos(){
      Transformacao4D tempMundoFilho = this.matriz.MultiplicarMatriz(this.matrizMundo);
      foreach(Objeto obj in this.objetosLista){
        obj.setMatrizMundo(tempMundoFilho);
        obj.atualizarMatrizFilhos();
      }
    }

    
    public Objeto(string rotulo)
    {
      this.rotulo = rotulo;
    }

    public BBox GetBBox(){
      return this.bBox;

      
    }
    protected abstract void atualizarBbox();

    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    protected float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }

    public void Desenhar()
    {
      GL.PushMatrix();
      GL.MultMatrix(matriz.ObterDados());
      DesenharAramado();
      for (var i = 0; i < objetosLista.Count; i++)
      { 
        objetosLista[i].Desenhar();
      }
      GL.PopMatrix();
    }
    protected abstract void DesenharAramado();
    public void FilhoAdicionar(Objeto filho)
    {
      Transformacao4D mundoFilho = this.matriz.MultiplicarMatriz(this.matrizMundo);
      filho.setObjetoPai(this);
      filho.setMatrizMundo(mundoFilho);
      this.objetosLista.Add(filho);
    }
    public void FilhoRemover(Objeto filho)
    {
      this.objetosLista.Remove(filho);
    }
    protected abstract void PontosExibir();
    public void PontosExibirObjeto()
    {
      PontosExibir();
      for (var i = 0; i < objetosLista.Count; i++)
      {
        objetosLista[i].PontosExibirObjeto();
      }
    }

    public void ExibeMatriz()
    {
      matriz.ExibeMatriz();
    }
    public void AtribuirIdentidade()
    {
      matriz.AtribuirIdentidade();
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }
    public  void TranslacaoXY(double tx, double ty)
    {
      Transformacao4D matrizTranslate = new Transformacao4D();
      matrizTranslate.AtribuirTranslacao(tx, ty, 0);
      matriz = matrizTranslate.MultiplicarMatriz(matriz);
      atualizarBbox();
    }

    public  void TranslacaoXYZ(double tx, double ty, double tz)
    {
      Transformacao4D matrizTranslate = new Transformacao4D();
      matrizTranslate.AtribuirTranslacao(tx, ty, tz);
      matriz = matrizTranslate.MultiplicarMatriz(matriz);
      atualizarBbox();

    }
    public void EscalaXY(double Sx, double Sy)
    {
      Transformacao4D matrizScale = new Transformacao4D();
      matrizScale.AtribuirEscala(Sx, Sy, 1.0);
      matriz = matrizScale.MultiplicarMatriz(matriz);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }

    public void EscalaXYZ(double Sx, double Sy, double Sz)
    {
      Transformacao4D matrizScale = new Transformacao4D();
      matrizScale.AtribuirEscala(Sx, Sy, Sz);
      matriz = matrizScale.MultiplicarMatriz(matriz);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }

    public void EscalaXYBBox(double escala)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;
      pontoPivo.ToString();

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpEscala.AtribuirEscala(escala, escala, 1.0);
      matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }
    public void EscalaXYZBBox(double escala)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;
      pontoPivo.ToString();

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpEscala.AtribuirEscala(escala, escala, escala);
      matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }

    public void RotacaoZ(double angulo)
    {
      matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
      matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }

    public void RotacaoX(double angulo)
    {
      matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
      matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }

    public void RotacaoY(double angulo)
    {
      matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
      matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }


    public void RotacaoZBBox(double angulo)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
      matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }

    public void RotacaoYBBox(double angulo)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
      matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);
      atualizarBbox();
      this.atualizarMatrizFilhos();
    }
  }
}