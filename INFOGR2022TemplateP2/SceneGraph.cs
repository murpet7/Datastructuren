using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    static class SceneGraph
    {
        public static void Render(Entity ent, MyApplication app)
        {
            if (ent.parent != null)
            {
                Mesh mesh = ent.mesh;
                Mesh parMesh = ent.parent.mesh;

                Matrix4 Tcamera = Camera.view;
                Matrix4 Tview = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
                
                mesh.globScale = mesh.scale;
                mesh.globRot = mesh.rot;
                mesh.globPos = mesh.pos;

                if (parMesh != null)
                {
                    mesh.globScale *= parMesh.globScale;
                    mesh.globRot += parMesh.globRot;
                    mesh.globPos += parMesh.globPos;
                }

                mesh.modelMatrix =
                    Matrix4.CreateScale(mesh.globScale) *
                    Matrix4.CreateRotationX(mesh.globRot.X) * Matrix4.CreateRotationY(mesh.globRot.Y) * Matrix4.CreateRotationZ(mesh.globRot.Z) *
                    Matrix4.CreateTranslation(mesh.globPos);

                mesh.Render(app.shader, mesh.modelMatrix * Tcamera * Tview, mesh.texture); //Moet nog worden veranderd
            }

            for (int i = 0; i < ent.children.Count; i++)
            {
                Render(ent.children[i], app); //Gaat recursief langs alle entities
            }
        }
    }

    class Entity
    {
        public Mesh mesh;
        public Entity parent;
        public List<Entity> children;

        public Entity(Mesh mesh, Entity parent = null)
        {
            children = new List<Entity>();
            this.mesh = mesh;
            this.parent = parent;
            if (parent != null) parent.children.Add(this);
        }
    }
}
