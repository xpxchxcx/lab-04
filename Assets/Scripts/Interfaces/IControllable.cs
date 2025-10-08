using UnityEngine;

public interface IControllable
{
   public bool IsInControl(); 
   public bool OnTakeControl(); 
   public void OnControlRemoved(); 
   public Vector2 TargetCharacterPosition(); 
   public Vector2 TargetLookAhead(); 
   public Vector3 WorldPosition(); 
   public float Aggression(); 
   public bool CameraSettingsChanged(); 
} 
