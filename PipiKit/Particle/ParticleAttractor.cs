using UnityEngine;

namespace ChenPipi.Particle
{

    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAttractor : MonoBehaviour
    {

        [SerializeField]
        public Transform target;

        [SerializeField]
        public float speed = 5f;

        private ParticleSystem m_ParticleSystem;

        private ParticleSystem.Particle[] m_Particles;

        protected void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
        }

        protected void Update()
        {
            float step = speed * Time.deltaTime;
            int count = m_ParticleSystem.GetParticles(m_Particles);
            Vector3 targetPos = target.position;
            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle particle = m_Particles[i];
                particle.position = Vector3.LerpUnclamped(particle.position, targetPos, step);
            }
            m_ParticleSystem.SetParticles(m_Particles, count);
        }

    }

}
