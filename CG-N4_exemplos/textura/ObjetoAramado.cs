using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace textura
{
  internal class ObjetoAramado : Objeto
  {
    protected List<Ponto4D> pontosLista = new List<Ponto4D>();

    protected override void atualizarBbox(){
      this.bBox = new BBox();
      Ponto4D primeiroPonto = this.pontosLista[0];
      this.bBox.Atribuir(primeiroPonto);
      foreach(Ponto4D ponto in this.pontosLista){
        this.bBox.Atualizar(ponto);
      }
    }

    public ObjetoAramado(string rotulo) : base(rotulo) { }

    protected override void DesenharAramado()
    {
      GL.LineWidth(base.PrimitivaTamanho);
      GL.PointSize(base.PrimitivaTamanho);
      GL.Color3(this.cor);
      GL.Begin(base.PrimitivaTipo);
      foreach (Ponto4D pto in pontosLista)
      { 
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
    }

    protected void PontosAdicionar(Ponto4D pto)
    {
      pontosLista.Add(pto);
    }

    protected void PontosRemoverTodos()
    {
      pontosLista.Clear();
    }

    protected override void PontosExibir()
    {
      Console.WriteLine("__ Objeto: " + base.rotulo);
      for (var i = 0; i < pontosLista.Count; i++)
      {
        Console.WriteLine("P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]");
      }
    }
  }
}