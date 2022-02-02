using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct VertexPositionNormalTexture
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TextureCoordinate;
    //public static readonly VertexDeclaration VertexDeclaration;
    public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
    {
        this.Position = position;
        this.Normal = normal;
        this.TextureCoordinate = textureCoordinate;
    }

    public override int GetHashCode()
    {
        // TODO: FIc gethashcode
        return 0;
    }

    public override string ToString()
    {
        return string.Format("{{Position:{0} Normal:{1} TextureCoordinate:{2}}}", new object[] { this.Position, this.Normal, this.TextureCoordinate });
    }

    public static bool operator ==(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
    {
        return (((left.Position == right.Position) && (left.Normal == right.Normal)) && (left.TextureCoordinate == right.TextureCoordinate));
    }

    public static bool operator !=(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() != base.GetType())
        {
            return false;
        }
        return (this == ((VertexPositionNormalTexture)obj));
    }
}