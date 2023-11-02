﻿using Microsoft.Xna.Framework;
using System;

namespace SnakeGameSource.Components.Colliders
{
    internal class SquareCollider : Collider
    {
        public override float GetDistanceToEdge(Vector2 position)
        {
            Vector2 VectorToCollider = Vector2.Normalize(GetComponent<Transform>().Position - position);
            VectorToCollider = new Vector2(MathF.Abs(VectorToCollider.X), MathF.Abs(VectorToCollider.Y));
            Vector2 UnitVector = VectorToCollider.X > VectorToCollider.Y ? Vector2.UnitX : Vector2.UnitY;
            float cosBeetweenVectors = Vector2.Dot(UnitVector, VectorToCollider);

            return GetComponent<Transform>().Scale / 2 / cosBeetweenVectors;
        }
    }
}