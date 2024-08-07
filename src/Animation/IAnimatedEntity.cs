﻿using System.Numerics;

namespace ConsoleRayTracer;

interface IAnimatedEntity : IEntity
{
    void Update(float timeElapsed);
}

sealed record Animated<E, O, B, R>(
    E Entity,
    O Offset = default!,
    B Brightness = default!,
    R Reflectance = default!
) : IAnimatedEntity
    where E : IEntity
    where O : IAnimation<Vector3>
    where B : IAnimation<float>
    where R : IAnimation<float>
{
    Vector3 _offset = Offset.GetValue(0F);
    float _brightness = 1F + Brightness.GetValue(0F);
    float _reflectance = Reflectance.GetValue(0F);

    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        Entity.Hit(ray with { Origin = ray.Origin - _offset }, tMin, tMax) is HitRecord record
            ? record with
            {
                Point = record.Point + _offset,
                Brightness = record.Brightness + _brightness,
                Reflectance = record.Reflectance + _reflectance,
            }
            : null;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Entity.Illuminate(new Apply<I>(entity, -_offset), record with { Point = record.Point - _offset })
            * _brightness;

    public void Update(float timeElapsed)
    {
        _offset = Offset.GetValue(timeElapsed);
        _brightness = 1F + Brightness.GetValue(timeElapsed);
        _reflectance = Reflectance.GetValue(timeElapsed);
    }
}
