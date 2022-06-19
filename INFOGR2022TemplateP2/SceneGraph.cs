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
        public static void Render(Entity ent, Surface screen)
        {
            Mesh mesh = ent.mesh;
            Mesh parMesh = ent.parent.mesh;

            mesh.globScale = mesh.scale * parMesh.globScale;
            mesh.globRot = mesh.rot + parMesh.globRot;
            mesh.globPos = mesh.pos + parMesh.globPos;

            mesh.modelMatrix =
                Matrix4.CreateScale(mesh.globScale) *
                Matrix4.CreateRotationX(mesh.globRot.X) * Matrix4.CreateRotationX(mesh.globRot.Y) * Matrix4.CreateRotationX(mesh.globRot.Z) *
                Matrix4.CreateTranslation(mesh.globPos);

            mesh.Render(mesh.shader, mesh.modelMatrix * Tcamera * Tview, mesh.shader);

            foreach (Entity entity in ent.children) //Gaat recursief langs alle entities
            {
                Render(entity, screen);
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
            parent.children.Add(this);
        }
        
    }
}
